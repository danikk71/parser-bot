from aiogram import Router, types, F
from aiogram.filters import Command, CommandStart
from aiogram.fsm.context import FSMContext
from Bot.states.user_states import SearchStates
from Bot.services.db import get_product
from Bot.keyboards.keyboards import keyboardButtons

user_router = Router()


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

    results = get_product(product_name)
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
