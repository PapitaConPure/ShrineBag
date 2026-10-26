# Shrine Bag
Makes Shrines of Chance pick from a finite, shuffled bag of outcomes instead of being able to fail forever.

I made this mod because my luck sucks and the vanilla behavior sucks even more. It's server-side, but only the host needs the mod. The default values are what I personally use.

## Features
* Each shrine gets its own finite, shuffled bag of outcomes. The outcomes are consumed one by one as you pray. The number of bags per shrine is configurable.
* Each bag contains 2 *'reward'* outcomes (configurable) and 4 *'failure'* outcomes (configurable). The bags are shuffled independently.
* A shrine's last bag will get preemptively discarded if only *'failure'* outcomes remain. Bags other than the last can also get "trimmed" (configurable).
* Credit cost and weight multipliers in relation to vanilla for balancing (configurable).
* Internally forces vanilla reward/failure chance, so mod compatibility should be pretty good.

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

### Balancing
| Name                   | Type    | Default | Description |
|------------------------|---------|---------|-------------|     
| Credit Cost Multiplier | Factor  | `1.0`   | By how much should the credits required by the Stage Director to spawn a Shrine of Chance be multiplied. Vanilla is `x1`. |
| Weight Multiplier      | Factor  | `1.0`   | By how much should the weight used by the Stage Director be multiplied. A higher weight means the shrine is more likely to spawn over other interactables. Vanilla is `x1`. |

These settings exist for balance purposes, although they're usually better left untouched unless you use some pretty wild settings.

#### Stage Director
At the start of each stage, a "Stage Director" is put in charge of spawning all the interactables. For this, the director will spend as much of its budget as possible ("credits"). The interactables are chosen randomly, but some have a higher/lower "weight", meaning they'll get selected more/less often for spawning.

### Testing
| Name                   | Type    | Default | Description |
|------------------------|---------|---------|-------------|
| Spawn The Council      | Evil    | Off     | Spawns ***'The Council'*** on stage 1. |
| Council Size           | Integer | `5`     | Sets the number of members that make up ***'The Council'***. |

***'The Council'*** is an EVIL organization consisting of a configurable(*) circle of Shrines of Chance. ***EVIL***.

(*) Please note: the canonical size of ***'The Council'*** is 5. Members are: Frankie, Chicho, Paulie, Sebastian, and Maurice.

### Megumin
| Name                   | Type    | Default | Description |
|------------------------|---------|---------|-------------|
| Megumin                | Megumin | Megumin | Megumin     |

Megumin.

## Contact
* Discord: `papitaconpure` (please be nice or I'll cry)

## Compatibility notes
From what I've tested: `hurleyapieaye-FasterChances` and `sylvie-ProactiveChanceDoll` work just fine.

I haven't really tested compatibility with mods that make Shrines of Chance infinite, but you can also use this mod for that. You just gotta configure it ("Bag Gen. Mode" = **Infinite**).

### Nerd
The hook calls the original `ShrineChanceBehavior.AddShrineStack` method, temporarily forcing the `On.RoR2.ShrineChanceBehavior.failureChance` threshold. It also needs to update and persist `On.RoR2.ShrineChanceBehavior.maxPurchaseCount` to match the configured total reward count. This should preserve the functionality of most hooks that call the original method (maybe).

Any mod that replaces `AddShrineStack`, bypasses the vanilla failure check, changes `failureChance` in a weird way, or depends on `maxPurchaseCount` in a peculiar way may still conflict.

## Changelog
Read the thing on [Thunderstore](https://thunderstore.io/c/riskofrain2/p/PapitaConPure/ShrineBag/changelog) or [GitHub](https://github.com/PapitaConPure/ShrineBag/blob/master/CHANGELOG.md). Should be the same on both places unless I'm currently doing something on GitHub (never?????????)

## Additional Info
Build info [here](https://github.com/PapitaConPure/ShrineBag/blob/master/wik/build.md).

god i love megumin she's so cute she's so precious oooo what the fu
