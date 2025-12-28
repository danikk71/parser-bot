from aiogram import Router, types, F
from aiogram.filters import Command, CommandStart
from aiogram.fsm.context import FSMContext
from Bot.states.user_states import SearchStates
from Bot.services.db import *
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


@user_router.message(F.text == "Улюблені")
@user_router.callback_query(F.data == "back_fav")
async def favourites_list(event: types.Message | types.CallbackQuery):
    user_id = event.from_user.id

    if isinstance(event, types.Message):
        answer = event.answer
    else:
        answer = event.message.answer
        await event.message.delete()
        await event.answer()

    products = get_favourites_list(user_id)
    if not products:
        await answer("Улюблених ще немає!")
    else:
        first_page_products = products[:ITEMS_PER_PAGE]
        total_pages = (len(products) + ITEMS_PER_PAGE - 1) // ITEMS_PER_PAGE
        kb = pages_kb(first_page_products, 0, total_pages, str(user_id), "fav")
        await answer("<b>Улюблені</b>", reply_markup=kb, parse_mode="HTML")


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
        case "fav":
            all_products = get_favourites_list(product)
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
    data_parts = callback.data.split("_")
    source = data_parts[1]
    id = data_parts[2]

    product = get_product_by_id(id)

    if not product:
        await callback.answer("Продукт не знайдено")
        return

    is_fav = is_favourite(callback.from_user.id, id)
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
        f"<b>Ціна: {product['price']} грн</b>\n"
        f"<b>Тип: {product['type']}</b>\n"
        f"<b>Бренд: {product['brand']}</b>\n"
        f"{specs_text}"
    )

    try:
        await callback.message.answer_photo(
            photo=product["imageURL"],
            caption=text,
            parse_mode="HTML",
            reply_markup=product_btn(product, is_fav, back_to=source),
        )
    except Exception as ex:
        print(f"Помилка {ex}")
        await callback.message.answer(
            text=text,
            parse_mode="HTML",
            reply_markup=product_btn(product, is_fav, back_to=source),
        )
    await callback.answer()


@user_router.callback_query(F.data == "back")
async def delete_msg(callback: types.CallbackQuery):
    await callback.message.delete()


@user_router.callback_query(F.data.startswith("favorites_"))
async def do_favourites(callback: types.CallbackQuery):
    data_parts = callback.data.split("_")

    mode = data_parts[1]
    product_id = int(data_parts[2])
    source = data_parts[3]
    user_id = callback.from_user.id

    match mode:
        case "add":
            rows_added = add_to_favourites(product_id, user_id)
            if rows_added > 0:
                product = get_product_by_id(product_id)
                await callback.message.edit_reply_markup(
                    reply_markup=product_btn(product, is_favorite=True, back_to=source)
                )
                await callback.answer("Товар додано до улюблених!", show_alert=True)
            else:
                await callback.answer("Товар уже в улюблених!", show_alert=True)
        case "remove":
            rows_removed = remove_from_favourites(product_id, user_id)
            if rows_removed > 0:
                product = get_product_by_id(product_id)
                await callback.message.edit_reply_markup(
                    reply_markup=product_btn(product, is_favorite=False, back_to=source)
                )
                await callback.answer("Товар видалено з улюблених!", show_alert=True)
            else:
                await callback.answer(
                    "Товар уже видалений з улюблених!", show_alert=True
                )


@user_router.callback_query(F.data.startswith("history_"))
async def price_history(callback: types.CallbackQuery):
    id = callback.data.split("_")[1]
    pricelist = get_prices(id)
    text = "<b>Історія цін</b>\n"
    for p in pricelist:
        text += f"<b>{p['price']}грн - {p['date_recorded']}</b>\n"

    await callback.message.answer(text, parse_mode="HTML")
    await callback.answer()
