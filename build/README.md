# Screamer
A plugin for Kerbal Space Program that can display messages in-game, based on a configuration file.

### Why?
People asked for a way to display messages regarding incompatibilities of two mods, using the NEEDS syntax of ModuleManager. So here is one.

### How?
Simply create a new cfg in your GameData, and edit the main cfg that is shipped using ModuleManager

```
@SCREAMER
{
    Scream
    {
        name = <Some name that is used internally. Should be unique.>
        once = <Whether it should be possible to display the message multiple times in a game session>
        delay = <How many secods should pass between the invokation of the message and it being actually displayed>
        condition = <Multiple conditions that need to be true in order for the message to show up>
        type = <Whether the message should be an entry in the log, a popup or a message on screen>
        style = <Where the message should be displayed on screen (ScreenMessage only)>
        duration = <How many seconds should the message be visible>        
        position = <where the message is located on screen (PopupDialog only)>
        title = <A headline for your message>
        message = <Your actual message. Newlines are done using @br>
        Actions <A list of button texts and actions (PopupDialog only)>
        {
            
        }
    }

    // Concrete example
    Scream
    {
        name = Magic
        once = false
        delay = 1.0
        condition = IsInSpaceCenter, IsSandbox 
        // Other options: https://github.com/StollD/Screamer/blob/master/src/Conditions.cs
        type = PopupDialog 
        // Other options: ScreenMessage, Debug
        position = 100, 100
        title = God
        message = Your system is broken.@br@brHave a nice day. 
        // Other variables: https://github.com/StollD/Screamer/blob/master/src/Variables.cs
        Actions
        {
            Action
            {
                name = OK
                actions = Dismiss
            }
            // Action
            // {
            //     name = Open Link
            //     actions = url(s):google.com
            //     // url -> http
            //     // urls -> https
            // }
            // Other Actions: https://github.com/StollD/Screamer/blob/master/src/Actions.cs
        }
    }
}
```

### Download?
Soon

### Forum thread?
No. Never. I don't want to deal with the forums right now, and I would just forget to maintain it like I did with KittopiaTech or Planetary Diversity.
Just bundle the release with your mods GameData folder and ship it, thats fine. But please keep it in a `GameData/Screamer` folder, so people don't install it multiple times.

Oh and ***please*** do not edit `GameData/Screamer/Config/Screamer.cfg` directly. Use ModuleManager.

### License?
License is MIT. Do what you want, but credit me please. 

