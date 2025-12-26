from aiogram import Router, types, F
from aiogram.filters import Command, CommandStart
from aiogram.fsm.context import FSMContext
from Bot.states.user_states import SearchStates
from Bot.services.db import get_product_by_name, get_product_by_type, get_product_by_id
from Bot.keyboards.keyboards import keyboardButtons, pages_kb, back_btn, product_btn
import json

user_router = Router()

ITEMS_PER_PAGE = 5


@user_router.message(CommandStart())
async def start_handler(message: types.Message):
    await message.answer("Обери дію!", reply_markup=keyboardButtons())


@user_router.message(Command("search"))
@user_router.message(F.text == "Шукати товар")
async def search_name_handler(message: types.Message, state: FSMContext):
    await message.answer("Введи назву продукту:")
    await state.set_state(SearchStates.waiting_for_product_name)


@user_router.message(SearchStates.waiting_for_product_name)
async def search_input(message: types.Message, state: FSMContext):
    product_name = message.text

    results = get_product_by_name(product_name)
    if not results:
        await message.answer("Продукт не знайдено!")
    else:
        first_page_products = results[:ITEMS_PER_PAGE]
        total_pages = (len(results) + ITEMS_PER_PAGE - 1) // ITEMS_PER_PAGE
        kb = pages_kb(first_page_products, 0, total_pages, product_name, "name")
        await message.answer("<b>Результати:</b>", reply_markup=kb, parse_mode="HTML")
    await state.clear()


@user_router.message(Command("type"))
@user_router.message(F.text == "Шукати за типом")
async def search_type_handler(message: types.Message, state: FSMContext):
    await message.answer("Введи тип продукту:")
    await state.set_state(SearchStates.waiting_for_product_type)


@user_router.message(SearchStates.waiting_for_product_type)
async def search_input(message: types.Message, state: FSMContext):
    product_type = message.text
    results = get_product_by_type(product_type)

    if not results:
        await message.answer("Тип не знайдено!")
    else:
        first_page_products = results[:ITEMS_PER_PAGE]
        total_pages = (len(results) + ITEMS_PER_PAGE - 1) // ITEMS_PER_PAGE
        kb = pages_kb(first_page_products, 0, total_pages, product_type, "type")
        await message.answer("<b>Результати:</b>", reply_markup=kb, parse_mode="HTML")
    await state.clear()


def get_page_content(products, page):
    start = page * ITEMS_PER_PAGE
    end = start + ITEMS_PER_PAGE
    current_products = products[start:end]

    text = f"<b>Сторінка {page+1}</b>\n\n"

    for p in current_products:
        if p["is_available"]:
            text += f"/id_{p['id']}<b>{p['name']}</b>\n"
    return text


@user_router.callback_query(F.data.startswith("page_"))
async def change_page(callback: types.CallbackQuery):
    data_parts = callback.data.split("_")
    mode = data_parts[1]
    page = int(data_parts[-1])
    product = "_".join(data_parts[2:-1])
    match mode:
        case "type":
            all_products = get_product_by_type(product)
        case "name":
            all_products = get_product_by_name(product)
        case _:
            await callback.answer("Невідомий режим", show_alert=True)
            return
    if not all_products:
        await callback.answer("Дані застаріли", show_alert=True)
        return

    start = page * ITEMS_PER_PAGE
    end = start + ITEMS_PER_PAGE
    current_products = all_products[start:end]

    total_pages = (len(all_products) + ITEMS_PER_PAGE - 1) // ITEMS_PER_PAGE
    kb = pages_kb(current_products, page, total_pages, product, mode)
    try:
        await callback.message.edit_text(
            "<b>Результати:</b>", reply_markup=kb, parse_mode="HTML"
        )
    except Exception:
        await callback.answer()


@user_router.callback_query(F.data.startswith("get_"))
async def get_product(callback: types.CallbackQuery):
    id = callback.data.split("_")[1]

    product = get_product_by_id(id)

    if not product:
        await callback.answer("Продукт не знайдено")
        return

    specs_text = ""
    try:
        specs = json.loads(product["specs"])
        if isinstance(specs, dict):
            for key, value in specs.items():
                specs_text += f"<b>{key}:</b> {value}\n"
        else:
            specs_text = str(specs)
    except json.JSONDecodeError:
        specs_text += f"{product['specs']}"

    text = (
        f"<b>Детальна інформація:</b>\n\n"
        f"<b>{product['name']}</b>\n"
        f"Ціна: {product['price']} грн\n"
        f"Тип: {product['type']}\n"
        f"Бренд: {product['brand']}\n"
        f"{specs_text}"
    )

    try:
        await callback.message.answer_photo(
            photo=product["imageURL"],
            caption=text,
            parse_mode="HTML",
            reply_markup=product_btn(product["url"]),
        )
    except Exception as ex:
        print(f"Помилка {ex}")
        await callback.message.answer(
            text=text,
            parse_mode="HTML",
            reply_markup=product_btn(product["url"]),
        )
    await callback.answer()


@user_router.callback_query(F.data == "back")
async def delete_msg(callback: types.CallbackQuery):
    await callback.message.delete()
