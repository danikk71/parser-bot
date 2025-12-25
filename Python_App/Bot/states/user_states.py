from aiogram.fsm.state import StatesGroup, State


class SearchStates(StatesGroup):
    waiting_for_product_name = State()
    waiting_for_product_type = State()
