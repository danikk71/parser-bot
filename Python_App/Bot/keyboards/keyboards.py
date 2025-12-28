from aiogram import types
from aiogram.types import ReplyKeyboardMarkup, KeyboardButton, InlineKeyboardMarkup
from aiogram.utils.keyboard import ReplyKeyboardBuilder, InlineKeyboardBuilder


def keyboardButtons() -> ReplyKeyboardMarkup:
    builder = ReplyKeyboardBuilder()

    builder.add(KeyboardButton(text="Шукати товар"))
    builder.add(KeyboardButton(text="Шукати за типом"))
    builder.add(KeyboardButton(text="Улюблені"))

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

    source = "fav" if mode == "fav" else "catalog"

    for p in products_on_page:
        name = p["name"]
        if len(name) > 48:
            name = f"{name[:48]}..."
        builder.button(
            text=f"{name} - {p['price']}грн",
            callback_data=f"get_{source}_{p['id']}",
        )
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
    builder.row(types.InlineKeyboardButton(text="Повернутись", callback_data="back"))
    return builder.as_markup()


def back_btn():
    builder = InlineKeyboardBuilder()
    builder.button(text="Повернутись", callback_data="back")
    return builder.as_markup()


def product_btn(product: dict, is_favorite: bool, back_to: str = "catalog"):
    builder = InlineKeyboardBuilder()
    changes = False
    if product.get("url"):
        builder.button(text="Детальніше", url=product["url"])
    if is_favorite:
        builder.button(
            text="Видалити з улюблених",
            callback_data=f"favorites_remove_{product['id']}_{back_to}",
        )
        changes = True
    else:
        builder.button(
            text="Додати до улюблених",
            callback_data=f"favorites_add_{product['id']}_{back_to}",
        )
    builder.button(
        text="Повернутись",
        callback_data="back_fav" if back_to == "fav" and changes == True else "back",
    )
    builder.adjust(1)
    return builder.as_markup()
