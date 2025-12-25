from aiogram import Router, types
from aiogram.filters import Command, CommandStart
from Bot.keyboards.keyboards import keyboardButtons

user_router = Router()


@user_router.message(CommandStart())
async def start_handler(message: types.Message):
    await message.answer("Обери дію!", reply_markup=keyboardButtons())
