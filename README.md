# PowerPlanSwitcher
A Playnite Plugin for changing the Windows power plan.

# Why should I use it?
The main reason is to give the user better control over their computer performance settings. The default Power Plan of Windows is the Balanced Profile, which is a mix of power saving and high performance settings. The main characteristic of the Balanced Power Plan is to enable CPU throttling for lower demanding workloads, while allowing the CPU to go full throttle if needed. However, sometimes this behavior may result in subpar gaming performance, with stuttering and FPS fluctuations. These issues were exacerbated with the latest KB5000842, KB5001330, KB5003173 and the 21H1 updates for Windows, which resulted in [many users facing severe performance issues](https://www.reddit.com/r/Windows10/comments/mqvhpa/kb5001330_bad_gaming_performance/) and [NVIDIA itself suggesting the affected users to uninstall the recent Windows Updates](https://www.windowscentral.com/nvidia-staff-says-gamers-should-uninstall-latest-windows-10-update-fix-issues). The workaround suggested by Microsoft was to use the High Performance Power Plan setting in Windows, thus why I've made this plugin.

# What it does?
This simple plugin will automatically switch the Windows Power Plan before launching any games through Playnite, and restore to the default Power Plan after the game is closed.

# How to use?
The default behavior of this plugin is to switch between two Power Plan settings (specified by the user) upon launching and closing games. You can also ignore games for switching the Power Plan according to their Source. The required Plugin configuration settings are:
1. **Default Power Plan GUID**: this is the default power plan GUID in your computer, to which this Plugin will switch back to upon closing games.
2. **Gaming Power Plan GUID**: this is the power plan this Plugin will switch to upon launching games.
3. *Ignored Sources*: this is a comma separated list of the Sources ignored by this Plugin for switching the Power Plans.

# How do I find my Power Plan GUID?
You can find your Power Plan GUID in Windows using the *Command Prompt*:
1. Open the Command Prompt;
2. Type in `powercfg /l`;

This is an output example of running the `powercfg /l` command:
```
powercfg /l

Existing Power Schemes (* Active)
-----------------------------------
Power Scheme GUID: 381b4222-f694-41f0-9685-ff5bb260df2e  (Balanced) *
Power Scheme GUID: 8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c  (High performance)
Power Scheme GUID: a1841308-3541-4fab-bc81-f71556f20b4a  (Power saver)
Power Scheme GUID: e9a42b02-d5df-448d-aa00-03f14749eb61  (Ultimate Performance)
Power Scheme GUID: f8e3807e-a6a3-4000-8c67-2600ed46d70b  (Gaming)
```

What you want is the long alphanumeric string, i.e., *381b4222-f694-41f0-9685-ff5bb260df2e* is the GUID for the *Balanced* Power Plan in this example.
