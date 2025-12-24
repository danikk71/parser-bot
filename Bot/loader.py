import sqlite3
import json
import os
from datetime import datetime

CURRENT_DIRECTORY = os.path.dirname(os.path.abspath(__file__))
DATA_DIRECTORY = os.path.join(CURRENT_DIRECTORY, "..", "Data")
JSON_PATH = os.path.join(DATA_DIRECTORY, "latest.json")
DB_PATH = os.path.join(DATA_DIRECTORY, "ActualProducts.db")


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
    conn.commit()
    return conn


def load_data():
    if not os.path.exists(JSON_PATH):
        print(f"Файл {JSON_PATH} не знайдено!")
        return
    with open(JSON_PATH, "r", encoding="utf-8") as f:
        products = json.load(f)

    conn = databases_init()
    cursor = conn.cursor()

    now = f"{datetime.now().year}-{datetime.now().month}-{datetime.now().day}"

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
                now,
            ),
        )
        cursor.execute("SELECT id FROM Products WHERE url = ?", (url,))
        result = cursor.fetchone()

        if result and is_available == True:
            product_id = result[0]

            cursor.execute(
                "SELECT 1 FROM PriceHistory WHERE product_id = ? AND date_recorded = ?",
                (product_id, now),
            )
            is_recorded_today = cursor.fetchone()
            if is_recorded_today is None:
                cursor.execute(
                    """
                    INSERT INTO PriceHistory (product_id, price, date_recorded)
                    VALUES (?, ?, ?)
                    """,
                    (product_id, price, now),
                )
    conn.commit()
    conn.close()


def erase_db():  # для тестів
    conn = sqlite3.connect(DB_PATH)
    cursor = conn.cursor()
    cursor.executescript(
        """
        DROP TABLE IF EXISTS PriceHistory;
        DROP TABLE IF EXISTS Products;
    """
    )
    print("db erased")
    conn.commit()
    conn.close()


if __name__ == "__main__":
    load_data()
    print("db created")
