# Eve Switcher
Simple configurable hotkeys to switch between active Eve Online clients. Supports setting a hotkey to cycle between login screens which is useful when mass logging in. Also supports multiple characters on the same hotkey.

## Hotkey Format

Hotkeys must be specified using Microsoft [Key Enum](https://docs.microsoft.com/en-us/dotnet/api/system.windows.input.key?view=net-5.0). For example: `Ctrl+Shift+F1`

## Multiple Characters Per Hotkey

To support multiple characters per hotkey, set the configuration like so:

```
"Hotkeys": {
    "F1": [
      "Character1",
      "Character2
    ]
  }
```

Eve Switcher will remember the last active character on a given hotkey and swap back to that character first.

## Troubleshooting

Note that global hotkeys cannot be registered by multiple applications. A hotkey will fail to register if another application is already using it.