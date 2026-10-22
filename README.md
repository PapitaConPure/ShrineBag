# Shrine Bag
Makes Shrines of Chance pick from a finite bag of outcomes instead of being able to fail forever.

I made this mod because my luck sucks and the vanilla behavior sucks even more. The default values are what I personally use.

## Features
* Each shrine gets its own finite, shuffled bag of outcomes. The amount of bags per shrine is configurable.
* Each bag contains 2 *'reward'* outcomes (configurable) and 4 *'failure'* outcomes (configurable). The bags are shuffled independently.
* Vanilla reward/failure logic remains pretty much untouched. The mod forces the failure chance after a shrine takes an outcome from its bag and updates the maximum purchases to match the configured total rewards. Compatibility should be pretty good.
* After the last bag is exhausted, that shrine remains unavailable even if you don't hit the vanilla reward limit of 2. Likewise, if the configured number of rewards surpasses that of vanilla, the Shrine will remain active until all its bags are empty.
* A shrine's last bag will get preemptively discarded if only *'failure'* outcomes remain.
* If you so desire, you can multiply the credit cost of spawning the shrine in relation to vanilla for balancing.

## Configuration
### General
| Name                   | Type    | Default | Description |
|------------------------|---------|---------|-------------|                                        
| Enabled                | Toggle  | On      | Turn this thing on or off (or maybe). |
| Total Rewards          | Integer | `2`     | The number of *'reward'* outcomes to put in each bag. |
| Max. Failures          | Integer | `4`     | The number of *'failure'* outcomes to put in each bag. Failures after the final reward are trimmed out. |
| Bag Gen. Mode          | Choice  | Exact   | The method used to generate bags. Works together with "Bag Gen. Count" |
| Bag Gen. Count         | Integer | `1`     | Number of bags used by the generation mode. |
| Only Trim Last Bag     | Toggle  | On      | When enabled, only the final bag of a shrine is trimmed. When disabled, every bag is. |
| Credit Cost Multiplier | Factor  | `1.0`   | By how much should the mod multiply the credits required by the stage director to spawn a Shrine of Chance. For balance purposes, although usually better left untouched. |

For context: the default behavior of 2 rewards vs. 4 failures makes each bag have 6 outcomes in total. This means that the worst-case scenario consists of 6 purchases: 4 failures first and then 2 rewards. The best-case scenario would still be 2 rewards up front.

#### Bag Generation
There are three methods of bag generation related to "Bag Gen. Count" (`N`), of which one is selected with "Bag Gen. Mode":
* **Exact:** each shrine holds exactly `N` bags.
* **Random:** each shrine randomly holds 1 to `N` bags.
* **Infinite:** every shrine holds infinite bags! `N` doesn't matter.

#### Trimming
Trimming removes every *'failure'* outcome that comes after a bag's last *'reward'* outcome.

The "Only Trim Last Bag" setting is only relevant when you configure multiple bags per shrine:
* **On:** Only trims the last bag of shrines. It keeps bag sizes consistent before the last one, resulting in more/wider "gaps" between rewards.
* **Off:** Trims every bag. Makes in-between bags that would otherwise end with one or more failures smaller.

When "Bag Gen. Mode" is set to **Infinite**, "Only Trim Last Bag" determines if trimming happens at all, as there is no last bag there.

### Testing
| Name                   | Type    | Default | Description |
|------------------------|---------|---------|-------------|
| Spawn The Council      | Toggle  | Off     | Spawns ***'The Council'*** on stage 1. |
| Council Size           | Integer | `5`     | Sets the number of members that make up ***'The Council'***. |

***'The Council'*** is an EVIL organization consisting of a configurable(**) circle of Shrines of Chance. ***EVIL***.

(*) Please note: the canonical size of ***'The Council'*** is 5. Members are: Frankie, Chicho, Paulie, Sebastian, and Maurice.

### Megumin
| Name                   | Type    | Default | Description |
|------------------------|---------|---------|-------------|
| Megumin                | Megumin | Megumin | Megumin     |

Megumin.

## Contact
* Discord: `papitaconpure` (please be nice or I'll cry)

## Compatibility notes
The hook calls the original `ShrineChanceBehavior.AddShrineStack` method, temporarily forcing the `On.RoR2.ShrineChanceBehavior.failureChance` threshold. It also needs to update and persist `On.RoR2.ShrineChanceBehavior.maxPurchaseCount` to match the configured total reward count. This should preserve the functionality of most hooks that call the original method (maybe).

Any mod that replaces `AddShrineStack`, bypasses the vanilla failure check, changes `failureChance` in a weird way, or depends on `maxPurchaseCount` in a peculiar way may still conflict.

## Build
Remember to check the .csproj and replace all applicable paths with the ones that match your install.

### Risk of Options
There's a compile flag for Risk of Options integration. It's set to `true` by default.

To compile with Risk of Options, place `RiskOfOptions.dll` at `<BepInExDir>/plugins/Rune580-Risk_Of_Options/RiskOfOptions/` and build the thing.

If for some reason you don't want that, build the thing with `-p:EnableRiskOfOptions=false`.

## Changelog
* **v1.1.0** - Added configurable amount and trimming of bags per shrine, stage director "Credit Cost Multiplier" option and ***'The Council'*** (EVIL).
* **v1.0.2** - Fixed some README things. Nothing ever happens.
* **v1.0.1** - Fixed a bug related to the "Total Rewards" behavior when pinging shrines.
* **v1.0.0** - I gave birth to this. pregnant emoji

## Additional Information
god i love megumin she's so cute she's so precious oooo what the fu
