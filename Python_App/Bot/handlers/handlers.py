from aiogram import Router, types, F
from aiogram.filters import Command, CommandStart
from aiogram.fsm.context import FSMContext
from Bot.states.user_states import SearchStates
from Bot.services.db import get_product_by_name, get_product_by_type
from Bot.keyboards.keyboards import keyboardButtons, pages_kb

user_router = Router()

ITEMS_PER_PAGE = 10


@user_router.message(CommandStart())
async def start_handler(message: types.Message):
    await message.answer("Обери дію!", reply_markup=keyboardButtons())


@user_router.message(Command("search"))
@user_router.message(F.text == "Шукати товар")
async def search_handler(message: types.Message, state: FSMContext):
    await message.answer("Введи назву продукту:")
    await state.set_state(SearchStates.waiting_for_product_name)


@user_router.message(SearchStates.waiting_for_product_name)
async def search_input(message: types.Message, state: FSMContext):
    product_name = message.text

    results = get_product_by_name(product_name)
    if not results:
        await message.answer("Продукт не знайдено!")
    else:
        for product in results:
            await message.answer(
                f"<b>{product['name']}</b>\n"
                f"Ціна: {product['price']}\n"
                f"<a href='{product['url']}'>Посилання</a>",
                parse_mode="HTML",
            )
    await state.clear()


@user_router.message(Command("type"))
@user_router.message(F.text == "Шукати за типом")
async def search_handler(message: types.Message, state: FSMContext):
    await message.answer("Введи тип продукту:")
    await state.set_state(SearchStates.waiting_for_product_type)


@user_router.message(SearchStates.waiting_for_product_type)
async def search_input(message: types.Message, state: FSMContext):
    product_type = message.text

    results = get_product_by_type(product_type)
    if not results:
        await message.answer("Тип не знайдено!")
    else:
        total_pages = (len(results) + ITEMS_PER_PAGE - 1) // ITEMS_PER_PAGE
        text = get_page_content(results, 0)
        kb = pages_kb(0, total_pages, product_type)
        await message.answer(text, reply_markup=kb, parse_mode="HTML")
    await state.clear()


def get_page_content(products, page):
    start = page * ITEMS_PER_PAGE
    end = start + ITEMS_PER_PAGE
    current_products = products[start:end]

    text = f"<b>Сторінка {page+1}</b>\n\n"

    for p in current_products:
        if p["is_available"]:
            text += f"<b>{p['name']}</b>\n"
    return text


@user_router.callback_query(F.data.startswith("page_"))
async def change_page(callback: types.CallbackQuery):
    data_parts = callback.data.split("_")
    page = int(data_parts[-1])
    product_type = "_".join(data_parts[1:-1])
    all_products = get_product_by_type(product_type)
    if not all_products:
        await callback.answer("Дані застаріли", show_alert=True)
        return

    total_pages = (len(all_products) + ITEMS_PER_PAGE - 1) // ITEMS_PER_PAGE
    text = get_page_content(all_products, page)
    kb = pages_kb(page, total_pages, product_type)
    try:
        await callback.message.edit_text(text, reply_markup=kb, parse_mode="HTML")
    except Exception:
        await callback.answer()
