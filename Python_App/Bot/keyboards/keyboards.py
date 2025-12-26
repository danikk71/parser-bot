from aiogram import types
from aiogram.types import ReplyKeyboardMarkup, KeyboardButton, InlineKeyboardMarkup
from aiogram.utils.keyboard import ReplyKeyboardBuilder, InlineKeyboardBuilder


def keyboardButtons() -> ReplyKeyboardMarkup:
    builder = ReplyKeyboardBuilder()

    builder.add(KeyboardButton(text="Шукати товар"))
    builder.add(KeyboardButton(text="Шукати за типом"))
    builder.add(KeyboardButton(text="Ага"))
    builder.add(KeyboardButton(text="Угу"))

    builder.adjust(2)

    return builder.as_markup(resize_keyboards=True, one_time_keyboard=False)


def pages_kb(
    products_on_page: list,
    page: int,
    total_pages: int,
    product_type: str,
    mode: str,
) -> InlineKeyboardBuilder:
    builder = InlineKeyboardBuilder()

    for p in products_on_page:
        builder.button(text=f"{p['name']}", callback_data=f"get_{p['id']}")
    builder.adjust(1)

    nav_buttons = []
    if page > 0:
        nav_buttons.append(
            types.InlineKeyboardButton(
                text="⬅️", callback_data=f"page_{mode}_{product_type}_{page-1}"
            )
        )
    nav_buttons.append(
        types.InlineKeyboardButton(
            text=f"{page+1}/{total_pages}", callback_data="ignore"
        )
    )

    if page < total_pages - 1:
        nav_buttons.append(
            types.InlineKeyboardButton(
                text="➡️", callback_data=f"page_{mode}_{product_type}_{page+1}"
            )
        )
    builder.row(*nav_buttons)
    return builder.as_markup()


def back_btn():
    builder = InlineKeyboardBuilder()
    builder.button(text="Повернутись", callback_data="back")
    return builder.as_markup()


def product_btn(product_url: str):
    builder = InlineKeyboardBuilder()
    if product_url:
        builder.button(text="Детальніше", url=product_url)
    builder.button(text="Повернутись", callback_data="back")
    builder.adjust(1)
    return builder.as_markup()
