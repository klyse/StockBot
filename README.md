# StockBot
[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2Fklyse%2FStockBot.svg?type=shield)](https://app.fossa.com/projects/git%2Bgithub.com%2Fklyse%2FStockBot?ref=badge_shield)


Sends the stock info of the day before at 07:00 UTC.

Deployment on Azure Functions.

## Configuration Keys
* AlphavantageApiKey
* Telegram:ApiKey
*	Telegram:NotificationToken (must be the same as in the specified callback https://url?token={notificationToken}
* Timers:DailyStockSummary (every day at 7 UTC: 0 0 7 * * *)

## Stock info
Provided by: https://www.alphavantage.co/

Limited to 5 requests/min and 500 requests per day in free version

## Telegram Bot
Register using BotFahter: https://telegram.me/BotFather

## Debug
Start the Azure Storage Emulator before debugging.


## License
[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2Fklyse%2FStockBot.svg?type=large)](https://app.fossa.com/projects/git%2Bgithub.com%2Fklyse%2FStockBot?ref=badge_large)