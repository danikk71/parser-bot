from config import DB_PATH
import sqlite3


def get_product_by_name(name: str):
    conn = sqlite3.connect(DB_PATH)
    conn.row_factory = sqlite3.Row
    cursor = conn.cursor()

    search = f"%{name}%"

    cursor.execute(
        "SELECT * FROM Products WHERE name LIKE ? AND is_available = 1",
        (search,),
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
        (type,),
    )
    products = cursor.fetchall()
    conn.close()
    return [dict(row) for row in products]


def get_product_by_id(id: int):
    conn = sqlite3.connect(DB_PATH)
    conn.row_factory = sqlite3.Row
    cursor = conn.cursor()

    cursor.execute(
        "SELECT * FROM Products WHERE id LIKE ?",
        (id,),
    )
    product = cursor.fetchone()
    conn.close()
    if product:
        return dict(product)
    return None


def add_to_favourites(product_id: int, user_id: int):
    with sqlite3.connect(DB_PATH) as conn:
        cursor = conn.cursor()

        cursor.execute(
            """CREATE TABLE IF NOT EXISTS Favourites(
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    user_id INTEGER NOT NULL,
                    product_id INTEGER NOT NULL,
                    UNIQUE(user_id,product_id),
                    FOREIGN KEY(product_id) REFERENCES Products(id) ON DELETE CASCADE);"""
        )
        cursor.execute(
            "INSERT OR IGNORE INTO Favourites (user_id, product_id) VALUES (?, ?)",
            (user_id, product_id),
        )
        conn.commit()

    return cursor.rowcount > 0


def get_favourites_list(id: int):
    with sqlite3.connect(DB_PATH) as conn:
        conn.row_factory = sqlite3.Row
        cursor = conn.cursor()

        cursor.execute(
            """SELECT * FROM Favourites WHERE user_id LIKE ?""",
            (id,),
        )
        text = "<b>Улюблені:</b>\n"
        products = cursor.fetchall()
        for p in products:
            id = p["product_id"]
            cursor.execute("SELECT * FROM Products WHERE id LIKE ?", (id,))
            product = cursor.fetchone()
            name = product["name"]
            if len(name) > 48:
                name = f"{name[:48]}..."
            text += f"{name} - {product['price']}грн\n"
        return text
