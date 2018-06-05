# Telegram-Schedlr
1. Create your own Telegram bot with the Botfather bot.
2. Enter in your telegram bot ID into the BotClient variable
3. Add the bot as an admin to your required chat
4. Do /getchatid in this chat to get the chat ID of the chat you want to schedule the posts to send to. (You must also be an admin!)
5. Enter this chat ID (including the negative symbol) into the 'ChatId' const
6. Run the bot. If you are an admin in the specified chat ID, the following commands will work (In private message ONLY!)

preview - Preview a normal message in PM

previewhtml - Preview a HTML message in PM

previewmarkdown - Preview a Markdown message in PM

send - Send a normal message in chat

sendhtml - Send a HTML message in chat

sendmarkdown - Send a markdown message in chat

getchatid - Get the chat ID

addmessage - Add normal message to scheduler [Interval] [Message]

addmarkdownmessage - Add a markdown message to scheduler [Interval] [Message]

addhtmlmessage - Add a HTML message to scheduler [Interval] [Message]

listschedules - List all schedules

showscheduletext - Show schedule text for specified schedule ID - [ScheduleId]

setinterval - Set the interval of an existing scheduled message. Use listschedules to find schedule id [ScheduleId] [Interval]

setmsgtext - Set the text of an existing scheduled message. Use listschedules to find schedule id [ScheduleId] [Message]

deleteschedule - Delete specified schedule [ScheduleId]

