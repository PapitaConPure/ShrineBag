# Shrine Bag
Makes Shrines of Chance pick from a finite bag of outcomes instead of being able to fail forever.

I made this mod because my luck sucks and the vanilla behavior sucks even more. The default values are what I personally use.

## Features
* Each shrine gets its own finite bag of outcomes.
* Each bag contains 2 'reward' outcomes (configurable) and 6 'failure' outcomes (configurable).
* Bags are shuffled independently for each shrine.
* Vanilla reward/failure logic remains pretty much untouched. The mod simply forces a reward or failure after a shrine takes an outcome from its bag. Compatibility should be pretty good.
* After the bag is exhausted, that shrine remains unavailable even if you don't hit the vanilla purchase limit (2). Likewise, if the configured number of rewards surpasses that of vanilla, the Shrine will remain active until the bag is empty.
* A shrine will get preemptively disabled if only failure outcomes remain in its bag.

## Configuration
### General
* `Enabled`: turn this thing on or off (or maybe).
* `Total Rewards`: The number of 'reward' outcomes to put in each bag.
* `Max. Failures`: The number of 'failure' outcomes to put in each bag. Failures after the final reward are trimmed out.

For context: default behavior is 2 rewards vs. 6 failures, for a total bag size of 8. This means that the worst-case scenario consists of 8 purchases: 6 failures first and then 2 rewards. The best-case scenario would still be 2 rewards up front.

## Risk of Options
There's a compile flag for Risk of Options integration. My brain is very smooth so I might just hard-depend on the thing if I don't get it working. I'm so sorry for adding one more mod to your 300+ mod profile if that's the case (you already have RoO installed don't lie to me).

To compile with Risk of Options, place `RiskOfOptions.dll` at `<BepInExDir>/plugins/Rune580-Risk_Of_Options/RiskOfOptions/` and build the thing with `-p:EnableRiskOfOptions=true`.

## Compatibility notes
The hook calls the original `ShrineChanceBehavior.AddShrineStack` method, temporarily forcing its failure threshold. This should preserve the functionality of most hooks that call the original method.

Any mod that replaces `AddShrineStack`, bypasses the vanilla failure check, or changes `failureChance` in a weird way may still conflict.

## Contact
* Discord: `papitaconpure` (please be nice I'll cry)

## Additional Information
god i love megumin she's so cute she's so precious oooo what the fu
