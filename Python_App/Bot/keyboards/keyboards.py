from aiogram.types import ReplyKeyboardMarkup, KeyboardButton
from aiogram.utils.keyboard import ReplyKeyboardBuilder, InlineKeyboardBuilder


def keyboardButtons() -> ReplyKeyboardMarkup:
    builder = ReplyKeyboardBuilder()

    builder.add(KeyboardButton(text="Шукати товар"))
    builder.add(KeyboardButton(text="Шукати за типом"))
    builder.add(KeyboardButton(text="Ага"))
    builder.add(KeyboardButton(text="Угу"))

    builder.adjust(2)

    return builder.as_markup(resize_keyboards=True, one_time_keyboard=False)


def pages_kb(page: int, total_pages: int, product_type: str) -> InlineKeyboardBuilder:
    builder = InlineKeyboardBuilder()

    if page > 0:
        builder.button(text="⬅️", callback_data=f"page_{product_type}_{page-1}")
    builder.button(text=f"{page+1}/{total_pages}", callback_data="ignore")

    if page < total_pages - 1:
        builder.button(text="➡️", callback_data=f"page_{product_type}_{page+1}")

    return builder.as_markup()
