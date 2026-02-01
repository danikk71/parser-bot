import os
from dotenv import load_dotenv

CURRENT_DIRECTORY = os.path.dirname(os.path.abspath(__file__))
DATA_DIRECTORY = os.path.join(CURRENT_DIRECTORY, "..", "Data")
JSON_DIRECTORY = os.path.join(DATA_DIRECTORY, "Archive")
LATEST_JSON_PATH = os.path.join(DATA_DIRECTORY, "latest.json")
DB_PATH = os.path.join(DATA_DIRECTORY, "Products.db")

load_dotenv(os.path.join(CURRENT_DIRECTORY, ".env"))
BOT_TOKEN = os.getenv("TOKEN")
