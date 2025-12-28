from config import DB_PATH
import sqlite3


def get_product_by_name(name: str):
    conn = sqlite3.connect(DB_PATH)
    conn.row_factory = sqlite3.Row
    cursor = conn.cursor()

    search = f"%{name}%"

    cursor.execute(
        """
        SELECT * FROM Products 
        WHERE name LIKE ? 
        AND is_available = 1
        AND time_updated = (SELECT MAX(time_updated) FROM Products)""",
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
        """
        SELECT * FROM Products 
        WHERE type LIKE ? 
        AND is_available = 1
        AND time_updated = (SELECT MAX(time_updated) FROM Products)""",
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
        "SELECT * FROM Products WHERE id = ?",
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


def remove_from_favourites(product_id: int, user_id: int):
    with sqlite3.connect(DB_PATH) as conn:
        conn.row_factory = sqlite3.Row
        cursor = conn.cursor()

        cursor.execute(
            "DELETE FROM Favourites WHERE user_id = ? AND product_id = ?",
            (user_id, product_id),
        )
        conn.commit()
    return cursor.rowcount > 0


def get_favourites_list(id: int):
    with sqlite3.connect(DB_PATH) as conn:
        conn.row_factory = sqlite3.Row
        cursor = conn.cursor()

        cursor.execute(
            """SELECT Products.* FROM Products
            JOIN Favourites ON Products.id = Favourites.product_id
            WHERE Favourites.user_id = ?""",
            (id,),
        )
        products = cursor.fetchall()
    return [dict(row) for row in products]


def is_favourite(user_id: int, product_id: int):
    with sqlite3.connect(DB_PATH) as conn:
        conn.row_factory = sqlite3.Row
        cursor = conn.cursor()

        cursor.execute(
            "SELECT 1 FROM Favourites WHERE user_id = ? AND product_id = ?",
            (user_id, product_id),
        )
        return cursor.fetchone() is not None


def get_prices(product_id: int):
    with sqlite3.connect(DB_PATH) as conn:
        conn.row_factory = sqlite3.Row
        cursor = conn.cursor()

        cursor.execute(
            """ SELECT price, date_recorded
                FROM PriceHistory 
                WHERE product_id = ? 
                ORDER BY date_recorded ASC""",
            (product_id,),
        )
        products = cursor.fetchall()
        return [dict(row) for row in products]
