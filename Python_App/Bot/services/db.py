from config import DB_PATH
import sqlite3


def get_product_by_name(name: str):
    conn = sqlite3.connect(DB_PATH)
    conn.row_factory = sqlite3.Row
    cursor = conn.cursor()

    # search = f"%{name}%"

    cursor.execute(
        "SELECT * FROM Products WHERE name LIKE ?",
        (name,),
    )

    products = cursor.fetchall()
    conn.close()

    return [dict(row) for row in products]


def get_product_by_type(type: str):
    conn = sqlite3.connect(DB_PATH)
    conn.row_factory = sqlite3.Row
    cursor = conn.cursor()

    cursor.execute(
        "SELECT * FROM Products WHERE type LIKE ? AND is_available = 1",
        (type, True),
    )
    products = cursor.fetchall()
    conn.close()
    return [dict(row) for row in products]
