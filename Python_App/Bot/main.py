import asyncio
from aiogram import Bot, Dispatcher
from config import BOT_TOKEN


async def main():
    await print("hi")


if __name__ == "__main__":
    try:
        asyncio.run(main())
    except KeyboardInterrupt:
        print("exit")
