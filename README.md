# TelegramCommandBot
1. Short overview
2. Example use case
3. Setup Telegram bot
4. Available commands

##Short overview
Hey Guys,
this is my first GitHub project and my English might be not the very best, but I think the point of my tool is prety good. It lets you communicate to your Windows Computer by the chat messenger Telegram. 
For now there are three main approaches:
- Send CMD commands to your computer and get the output
- Ask your computer for current screenshot
- Ask your computer for a file at specific path (you can look through by CMD)

## Example use case
I had the idea for this project while I was at my job and wondered, if my long running download is ready. I used TeamViewer for this job, which works very good, but with limited mobile data on my phone I searched another solution.

With this project I am able to:

- look up screenshots until I know that the download is complete
- send a command to my computer to shut down

## Setup Telegram bot
1. Most easy is the setup on the [web surface of Telegram](https://web.telegram.org). 
2. Then you ether search for "BotFather" or visit this [link](https://web.telegram.org/#/im?p=@BotFather).
3. There you press on Start and then run the command `/newbot` and hit enter.
4. Now you will be asked for a name. Name it `TelegramCommanbBot` and hit enter.
5. Then you need a unique username with `_bot` at the end. Press enter again. You may need a couple tries to get a unique username, but after you will receive an answer with `HTTP API`. The following key is needed to run the bot.


To simplify the using you can also add predefined commands, so you don't have to enter the command but select from list. Run `setcommands` and select the just created bot. Now you can enter a list of your prefered commands like
```
screenshot - Get current screenshot
cmd - Run a CMD command
...
```

## Available commands
`/cmd <arguments>` runs the arguments on your computer CMD and returns you the result

`/screenshot` makes a screenshot on your computer and sents it to you. In settings you can specify wether you want it as a file, which is uncompressed, a compressed image by Telegram, or both.

`/file <filepath>` delivers you a file from a specified path on your computer.
