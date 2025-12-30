import matplotlib.pyplot as plt
import io


def create_price_plot(price_history: list):
    dates = [item["date_recorded"] for item in price_history]
    prices = [item["price"] for item in price_history]

    plt.figure(figsize=(10, 6))
    plt.plot(
        dates,
        prices,
        marker="o",
        linestyle="-",
        color="#2a9d8f",
        linewidth=2,
        markersize=6,
    )

    plt.fill_between(dates, prices, color="#2a9d8f", alpha=0.1)

    if prices:
        min_price = min(prices)
        max_price = max(prices)

        if min_price == max_price:
            margin = max_price * 0.05 if max_price != 0 else 10
            plt.ylim(max_price - margin, max_price + margin)
        else:
            delta = max_price - min_price
            margin = max(delta * 0.1, max_price * 0.05)
            plt.ylim(min_price - margin, max_price + margin)

    plt.title("Динаміка цін", fontsize=16, fontweight="bold")
    plt.xlabel("Дата", fontsize=12)
    plt.ylabel("Ціна (грн)", fontsize=12)
    plt.grid(True, linestyle="--", alpha=0.5)

    plt.xticks(rotation=45)

    for i, price in enumerate(prices):
        offset = (plt.ylim()[1] - plt.ylim()[0]) * 0.02
        plt.text(
            dates[i], price + offset, f"{price}", ha="center", va="bottom", fontsize=9
        )

    plt.tight_layout()

    buf = io.BytesIO()
    plt.savefig(buf, format="png")
    buf.seek(0)
    plt.close()

    return buf
