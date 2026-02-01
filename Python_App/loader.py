import sqlite3
import json
import os
from config import *
from datetime import datetime


def databases_init():
    conn = sqlite3.connect(DB_PATH)
    cursor = conn.cursor()

    cursor.execute(
        """
        CREATE TABLE IF NOT EXISTS Products(
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        url TEXT UNIQUE,
        type TEXT,
        name TEXT,
        brand TEXT,
        price INTEGER,
        imageURL TEXT,
        is_available BOOLEAN,
        specs TEXT,
        time_updated DATETIME)
        """
    )
    cursor.execute(
        """
        CREATE TABLE IF NOT EXISTS PriceHistory(
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            product_id INTEGER,
            price INTEGER,
            date_recorded DATETIME,
            FOREIGN KEY(product_id) REFERENCES Products(id)
        )
        """
    )
    cursor.execute(
        """CREATE TABLE IF NOT EXISTS Favourites(
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                user_id INTEGER NOT NULL,
                product_id INTEGER NOT NULL,
                UNIQUE(user_id,product_id),
                FOREIGN KEY(product_id) REFERENCES Products(id) ON DELETE CASCADE);"""
    )
    conn.commit()
    return conn


def load_data(PATH: str, cursor: sqlite3.Cursor):
    if not os.path.exists(PATH):
        print(f"Файл {PATH} не знайдено!")
        return
    with open(PATH, "r", encoding="utf-8") as f:
        products = json.load(f)

    file = os.path.basename(PATH)
    filedate = os.path.splitext(file)[0]
    if filedate == "latest":
        date = datetime.now().strftime("%Y-%m-%d")
    else:
        date_obj = datetime.strptime(filedate, "%Y-%m-%d")
        date = date_obj.strftime("%Y-%m-%d")

    KEYS = ["ProductURL", "ImageURL", "$type", "Name", "Brand", "Price", "IsAvailable"]

    for item in products:
        url = item.get("ProductURL")
        imageURL = item.get("ImageURL")

        prod_type = item.get("$type")
        name = item.get("Name")
        brand = item.get("Brand")
        price = item.get("Price")
        is_available = item.get("IsAvailable")

        specs = item.copy()
        for key in KEYS:
            if key in specs:
                del specs[key]
        specs_json = json.dumps(specs, ensure_ascii=False)

        if not url:
            print(f"Пропущено товар без URL: {name}")
            continue

        cursor.execute(
            """
            INSERT INTO Products (url, type, name, brand, price, imageURL, is_available, specs, time_updated)
            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)
            ON CONFLICT(url) DO UPDATE SET
                price = excluded.price,
                is_available = excluded.is_available,
                imageURL = excluded.imageURL,
                time_updated = excluded.time_updated
        """,
            (
                url,
                prod_type,
                name,
                brand,
                price,
                imageURL,
                is_available,
                specs_json,
                date,
            ),
        )
        cursor.execute("SELECT id FROM Products WHERE url = ?", (url,))
        result = cursor.fetchone()

        if result and is_available == True:
            product_id = result[0]

            cursor.execute(
                "SELECT 1 FROM PriceHistory WHERE product_id = ? AND date_recorded = ?",
                (product_id, date),
            )
            is_recorded_today = cursor.fetchone()
            if is_recorded_today is None:
                cursor.execute(
                    """
                    INSERT INTO PriceHistory (product_id, price, date_recorded)
                    VALUES (?, ?, ?)
                    """,
                    (product_id, price, date),
                )


def erase_db():  # для тестів
    with sqlite3.connect(DB_PATH) as conn:
        cursor = conn.cursor()
        cursor.executescript(
            """
            DROP TABLE IF EXISTS PriceHistory;
            DROP TABLE IF EXISTS Products;
            DROP TABLE IF EXISTS Favourites
        """
        )
        print("db erased")
        conn.commit()


def load_all_jsons():
    with databases_init() as conn:
        cursor = conn.cursor()
        try:
            if os.path.exists(JSON_DIRECTORY):
                for file in os.listdir(JSON_DIRECTORY):
                    if file.endswith(".json"):
                        JSON_PATH = os.path.join(JSON_DIRECTORY, file)
                        load_data(JSON_PATH, cursor)
            else:
                print("directory not found")
            conn.commit()
        except Exception as ex:
            print("error in loading jsons...")
            conn.rollback()


if __name__ == "__main__":
    with databases_init() as conn:
        cursor = conn.cursor()
        load_data(LATEST_JSON_PATH, cursor)
        conn.commit()
    # erase_db()
    # load_all_jsons()
    print("db updated")
