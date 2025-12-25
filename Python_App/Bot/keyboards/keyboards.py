from aiogram.types import ReplyKeyboardMarkup, KeyboardButton
from aiogram.utils.keyboard import ReplyKeyboardBuilder


def keyboardButtons() -> ReplyKeyboardMarkup:
    builder = ReplyKeyboardBuilder()

    builder.add(KeyboardButton(text="Шукати товар"))
    builder.add(KeyboardButton(text="Я не придумав ще"))
    builder.add(KeyboardButton(text="Ага"))
    builder.add(KeyboardButton(text="Угу"))

    builder.adjust(2)

    return builder.as_markup(resize_keyboards=True, one_time_keyboard=False)
