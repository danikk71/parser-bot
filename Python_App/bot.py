import asyncio
from aiogram import Bot, Dispatcher
from config import BOT_TOKEN
from Bot.handlers.handlers import user_router

bot = Bot(token=BOT_TOKEN)
dp = Dispatcher()


async def main():
    dp.include_router(user_router)
    await dp.start_polling(bot)


if __name__ == "__main__":
    try:
        asyncio.run(main())
    except KeyboardInterrupt:
        print("exit")
