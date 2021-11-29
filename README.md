# Rachel Discord Bot

[![CodeFactor][CodeFactorBadge]][CodeFactorRepository] 
[![Support Server][DiscordIcon]][DiscordInvite]
![Made with](https://img.shields.io/badge/Made%20with-Love%20%E2%9D%A4%EF%B8%8F-red)
[![Avatars by](https://img.shields.io/badge/Avatars%20by-aster__atheris-orange)][ArtistInstagram]
***
![Rachel by aster_atheris][RachelNormal]

## Table of Contents
- [Feature list][FeatureList]
	- [Banning and kicking users][BansAndKicks]
	- [Two warning systems (quantity- and point-based)][WarningSystems]
	- [Praise system][PraiseSystem]
	- [Point-based, XBOX-like  achievement system][AchievementSystem]
	- [Customizable guild (server) wise command prefix (default $)][CustomizableCommandPrefix]
	- [Customizable guild (server) wise user join and left message][UserJoinLeftMessage]
	- [Simple user leveling system][LevelingSystem]
	- [Moderation announcement system][ModerationAnnouncement]
	- [Raffle system][RaffleSystem]
	- [CYOA Player][CYOAPlayer]
- [Requirements][Requirements]
	- [Additional "requirements"][AdditionalRequirements]
	- [Why Rachel need these permissions and how she use them][PermissionsUsage]
- [Getting Started][GettingStarted]
	- [Basic Configuration][BasicConfiguration]
- [Command List][CommandList]
	- [Configuration Commands][ConfigurationCommands]
	- [Moderator commands][ModeratorCommands]
	- [User commands][UserCommands]
- [Currently supported languages][SupportedLanguages]
- [Support us][SupportUs]
- [Credits][Credits]
- [License][License]
- [Changelog][Changelog]
- [What I have learned working on this project][WhatIHaveLearned]
- [Information for contributors][InfoForContributors]

***
## Feature list
### Banning and kicking users
To ban user every staff member can use $ban command. They will need to ping user and reason of ban.

Rachel won't remove any messages of that user.

To kick user every staff member can use $kick command. They will need to ping user and reason of kick.

### Two warning systems (quantity- and point-based)
**Pre-Configuration:**

To fully utilize warning system you will need to have:
- Configured staff roles
- Configured ToS channel
- Configured punishment role

To configure staff roles server administrator will need to call `$ChangeStaffRoles {ping of roles}` or `$AddStaffRoles {ping of roles}` command.

To configure ToS channel server administrator will need to call `$ChangeToSChannel {ping of channel}` command.

To configure punishment role server administrator will need to call `$ChangePunishmentRole {ping of role}` command.

**Point-based system:**

To use this system server administrator will need to call `$TogglePointSystemWarns true` command.

To add autobanning feature after certain point threshold server administrator will need to call `$ChangeWarnPointsTillBan {how many points required to get banned. 0 - will disable this feature.}` command.

This feature is disabled by default.

To add auto giving punishment role feature after certain point threshold server administrator will need to call `$ChangeWarnPointsTillPunishment {how many points required to get punishment role. 0 - will disable this feature.}` command.

This feature is disabled by default.

Every staff member will be able to warn users with `$Warn {user ping} {warn power (number)} {warn reason}` command. 

**Quantity-based system:**

Default system.

To use this system server administrator could need to call `$TogglePointSystemWarns false` command.

To add autobanning feature after certain quantity threshold server administrator will need to call `$ChangeWarnCountTillBan {how many warnings are required to get banned. 0 - will disable this feature.}` command.


This feature is disabled by default.

To add auto giving punishment role feature after certain quantity threshold server administrator will need to call `$ChangeWarnCountTillPunishment {how many warnings are required to get punishment role. 0 - will disable this feature.}` command.

This feature is disabled by default.

Every staff member will be able to warn users with `$Warn {user ping} {warn reason}` command.

Every warn given in quantity based system has 20 points so it's easly convertable to point-based one.

**Relevant for both systems:**

To change warn duration (default 30 days) server administrator will need to call `$ChangeWarnDuration {new warn duration in days. 0 will disable warning system completely}`.

Every user can check each other warns using `$warns {optional user ping}`.

Every staff member can remove warning before it expires using `$remove warn {user ping} {warn id}` command (to get `{warn id}` staff member will need to call `$warns {user ping}` command and check leftmost number of warning to remove).

If staff members decides that warning would be too extreme punishment, he could use `$reprimand {user ping} {reprimand reason}`. It will send a DM message to user and will not have any more consequences.

In every warn and reprimand DM send to user will be direct link to server ToS channel. Make sure you have it configured, because if it's not configured correctly user will get a message that ends with: "Please read rules on #deleted-channel".

### Praise system
Every staff member will be able to praise users with `$Praise {user ping} {praise reason}` command.

Every user can check each other praises using `$praises {optional user ping}`.

Every staff member can remove praise using `$remove praise {user ping} {praise id}` command (to get `{praise id}` staff member will need to call `$praises {user ping}` command and check leftmost number of praise to remove).

### Point-based, XBOX-like  achievement system
Every staff member will be able to give user achievement with `$Achievement {user ping} {achievement points} {achievement text}` command.

Every user can check each other achievements using `$achievements {optional user ping}`.

Every staff member can remove achievement using `$remove achievement {user ping} {achievement id}` command (to get `{achievement id}` staff member will need to call `$achievements {user ping}` command and check leftmost number of achievement to remove).

### Customizable guild (server) wise command prefix (default $)
Server administrator can call `$ChangeGuildPrefix {new prefix}` command to change command prefix for this server.

### Customizable guild (server) wise user join and left message
Rachel has own user join and left messages.

Server administrator will need to call `$ChangeUsersJoiningChannel {ping of channel}` to enable welcome messages and `$ChangeUsersLeftChannel {ping of channel}` for user left notifications.

In order to change messages content administrator will need to call `$ChangeWelcomeMessage {new message content}` and `$ChangeUserLeftMessage {new message content}`.

User join/leave messages placeholders
- {0} - user mention
- {1} - user username
- {2} - user id
- {3} - guild (server) name

Example:

Message `User {1}({2}) left our server!` will be shown as `User Jurij98(331026920269414410) left our server!`

### Simple user leveling system
Every time user sends message he will get 50xp.

After user level up it is possible to give user role reward.

Every staff member will be able to add and remove role rewards using `$Add Level Role {role ping} {required lvl}` and `$Remove Level Role {role ping}` commands.

### Moderation announcement system
Moderation announcements are special messages that can be modified by any staff member.

To add new moderation announcement every staff member can use `$Create Announcement {channel where put announcement} {title of announcement}|{content of announcement}` command.
It is very important that first | character is end of title, so no spoilers in title (in content they are possible).

To update already existing announcement staff member will need to have discord message id of edited announcement.
In order to do that every staff member will need to enable "Developer Mode" in Discord Appearance settings, then right click on announcement message and "Copy ID".
Once staff member has message id he can use `$Update Announcement {message id} {new title}|{new content}` command.
Just like in announcement creation: First | character is end of title.

Announcement author will be changed to staff member who edited announcement most recently.

### Raffle system
In order to create new raffle every staff member can use `$Create Raffle {true - if you want users to enter raffle by themselfs, false - if you want full control over raffle} {raffle reward}`.

To add tickets to user every staff member can use `$Add Tickets To User {raffle id} {user ping} {tickets amount}` command that
will add specified ticket amount to user. Every staff member can use `$Add Tickets To Role {raffle id} {role ping} {tickets amount}` 
command that will add specified ticket amount to all users with role.

To roll winner of raffle every staff member can use `$Roll Raffle {raffle id}`.

Every user can try to enter a raffle using `$Join Raffle {raffle id}` command.
This command will add one ticket to raffle only if user didn't already enter it and staff member set raffle to open for all users.

To check raffles every user can use `$Show Raffles` command, that will show all raffles (ended ones will be crossed out).

To check specific raffle every user can use `$Show Raffle {raffle id}` command.

### CYOA Player
(In order for this feature to work you need to join our [Discord server][DiscordInvite])

To check list of available adventures every user need to use `$cyoa list` command. This command will return adventure code, it's language and minimal recommended age.

To choose adventure user want to play every user need to use `$cyoa choose adventure {adventure code (from $cyoa list)}`.

To play adventure every user need to use `$cyoa play`. It is recommended to use this in private channels (Rachel's prefix in private channels is $).

To make choices every user need to use `$cyoa make choice {choice id (from $cyoa play)`. It is recommended to use this in private channels.

To change MC gender and name (not every adventure supports this feature) every user could use `$cyoa change profile {true - if female|false - if male} {MC name}`.

To check their current adventure and MC each user can use `$cyoa profile`.

*If you want to create own stories:*
- *Download latest version of [our tool][CYOATool]*
	- *When creating new adventure make sure you put `2` as first character in `Created for app version` field.*
	- *During creation process please use discord markdown syntax, even if it is not supported via app. And don't use sounds.*
- *Once you finish*
	- *Go to `{CYOA tool}/Resources/Stories` path*
	- *Compress your entire story folder into .zip or .rar*
	- *Send us your compressed story folder via*
		- *E-mail: BajarzDevelopment@Gmail.com*
		- *Discord: DM to Jurij98#2750*

[Back to top][BackToTop]

***
## Requirements
There are no hard requirements but some commands won't work properly without additional permissions.

### Additional "requirements"
- Rachel should have administrator privileges on your server.
	- If you don't want to give Rachel administrator privileges, you can give her (some commands will require this permissions in order to work):
		- Ban members
		- Kick members
		- Manage roles
		- Manage channels
		- Manage messages *(all features will work without it)*
- Your server should have rules channel.

### Why Rachel need these permissions and how she use them
- Ban members
	- **$ban** command obviously won't work
	- **$warn** command will work but auto banning feature after reaching configurable threshold won't
- Kick members
	- **$kick** command obviously won't work
- Manage roles
	- **$warn** command will work but auto giving punishment role feature after reaching configurable threshold won't
	- **$add level role** command won't work because after reaching by user certain level Rachel would give user reward role
	- **$give role** command won't work because won't have permission to add role to user
	- **$remove role** command won't work because won't have permission to remove role from user
- Manage channels
	- **$unlock channel** command won't work because Rachel will modify role permissions to channels
	- **$lock channel** command won't work because Rachel will modify role permissions to channels
	- **$slowmode** command won't work because Rachel will modify channel slowmode duration
- Manage messages
	- **$poll** command will work, but Rachel won't remove user message with poll content
	- **$tictactoe** command will work, but Rachel won't remove user message with square choice
	
[Back to top][BackToTop]

***
## Getting Started

### Basic Configuration
1. Make sure your server meets Rachel requirements.
2. [Invite Rachel with administrator privileges][InviteRachelAdmin] or [invite her with only required permissions][InviteRachelBasic] or, if you want full control over Rachel permissions, [invite her without any permissions][InviteRachelPermless].
3. Make sure Rachel role is as high as possible in role hierarchy.
4. Call 2 commands:
	- $ChangeToSChannel **Required**
	- $AddStaffRoles **Required**
5. Enjoy!

[Back to top][BackToTop]

***
![Smiling Rachel by aster_atheris][RachelSmiling]
## Command List

### Configuration Commands

**All configuration commands require user to have administrator privileges.**

*Bolded commands are obligatory.*

| Command | Parameters | Outcome | Example |
| --- | --- | --- | --- |
| ChangeGuildPrefix | string representing new command prefix | Change guild command prefix | $ChangeGuildPrefix 12 |
| ChangeGuildLanguage | string representing [language ISO code][ISOCodes] | Change guild language | $ChangeGuildLanguage pl |
| **AddStaffRoles** | at least one SocketRole object (discord role) | Adds roles to staff roles | $AddStaffRoles @Owner @Admin @Moderator |
| ChangeStaffRoles | at least one SocketRole object (discord role) | Clears staff roles and adds specified roles to staff roles | $ChangeStaffRoles @Owner @Admin @Moderator |
| ChangeModerationChannel | Text channel object | Change where to log moderation commands output | $ChangeModerationChannel #server-staff |
| ChangeUsersJoiningChannel | Text channel object | Change where to welcome users | $ChangeUsersJoiningChannel #welcome-channel |
| ChangeWelcomeMessage | string representing new welcome message | Change welcome message (more detailed information in different section) | $ChangeWelcomeMessage User {1} joined our server! |
| ChangeUsersLeftChannel | Text channel object | Change where log user left messages | $ChangeUsersLeftChannel #left-channel |
| ChangeUserLeftMessage | string representing new user left message | Change user left message (more detailed information in different section) | $ChangeUserLeftMessage User {1}({2}) left our server! |
| ChangePunishmentRole | SocketRole object (discord role) | Change punishment role which user can get when exceeding warn limits | $ChangePunishmentRole @PunishmentRole |
| ChangePunishmentChannel | Text channel object | Change channel for users with punishment role | $ChangePunishmentChannel #punishment-channel |
| TogglePointSystemWarns | true or false | Use point- (true) or quantity- (default, false) based warn system | $TogglePointSystemWarns true |
| ChangeWarnDuration | Non-negative integer | Change how many days warns should be active (default 30 days) | $ChangeWarnDuration 7 |
| ChangeWarnCountTillBan | Non-negative integer | Change after how many active warns user will get automatically banned (0 will disable this feature) | $ChangeWarnCountTillBan 5 |
| ChangeWarnCountTillPunishment | Non-negative integer | Change after how many active warns user will get punishment role (0 will disable this feature) | $ChangeWarnCountTillPunishment 3 |
| ChangeWarnPointsTillBan | Non-negative integer | Change after how many points of active warns user will get automatically banned (0 will disable this feature) | $ChangePointsCountTillBan 100 |
| ChangeWarnPointsTillPunishment | Non-negative integer | Change after how many points of active warns user will get punishment role (0 will disable this feature) | $ChangePointsCountTillPunishment 50 |
| ChangeAnnouncementChannel | Text channel object | Change channel for announcements | $ChangeAnnouncementChannel #announcements |
| **ChangeToSChannel** | Text channel object | Change channel with server rules | $ChangeToSChannel #server-rules |

[Back to top][BackToTop]

### Moderator commands
**All moderation commands require user to have one of staff roles and some Rachel to have additional privileges (Banning members, kicking members, managing roles and channels).**

| Command | Aliases | Parameters | Outcome | Example |
| --- | --- | --- | --- | --- |
| Ban | --- | User object and string representing ban reason | Will ban user | $ban @Jurij98 ban reason |
| Kick | Wyrzuć, Wyrzuc | User object and string representing kick reason | Will kick user | $kick @Jurij98 kick reason |
| Praise | Pochwal | User object and string representing praise reason | Will add praise to user account | $praise @Jurij98 praise reason |
| Remove Praise | Usuń pochwałę, Usun pochwale | User object and integer representing praise id | Will remove praise from user account | $remove praise @Jurij98 1 |
| Reprimand | Upomnienie | User object and string representing reprimand reason | Will reprimand user | $Reprimand @Jurij98 reason |
| Warn | Ostrzeżenie, Ostrzezenie | User object and string representing warn reason | Will add warn to user account. If you use point-based warning system reason should start with non-negative integer | $warn @Jurij98 10 warn reason |
| Remove Warn | Usuń Ostrzeżenie, Usun Ostrzezenie | User object and integer representing warn id | Remove warn with specified id from user | $remove warn @Jurij98 23 |
| Achievement | Osiągnięcie, Osiagniecie | User object, integer representing value of achievement and string representing achievement | Will add achievement to user account | $achievement @Jurij98 20 achievement |
| Remove Achievement | Usuń Osiągnięcie, Usun Osiagniecie | User object and integer representing achievement id | Will remove achievement from user account | $remove achievement @Jurij98 1 |
| Unlock Channel | Odblokuj Kanał, Odblokuj Kanal | Role and at least one channel object | Will unlock channel for role with default permissions | $unlock channel @Role #channel #other-channel |
| Lock Channel | Zablokuj Kanał, Zablokuj Kanal | Role and at least one channel object | Will lock channel for role with default permissions | $lock channel @Role #channel #other-channel |
| Add Level Role | Dodaj Rolę Za Level, Dodaj Role Za Level | Role and non-negative integer representing level requirement | Will add role as leveling reward | $add level role @100lvlRole 100 |
| Remove Level Role | Usuń Rolę Za Level, Usun Role Za Level | Role object |  Will remove role from leveling rewards | $remove level role @100lvlRole |
| Create Announcement | Nowe Ogłoszenie, Nowe Ogloszenie | Channel object and announcement title and content divided by \| character | Will send a message with title and content on specified channel. | $Create Announcement #example-channel Lorem Ipsum|dolor sit amet. |
| Update Announcement | Zaktualizuj Ogłoszenie, Zaktualizuj Ogloszenie | Announcement message id and new announcement title and content divided by \| character | Will update announcement with new title and content. | $Update Announcement 803662621169418290 Lorem Ipsum|Lorem ipsum dolor sit amet. |
| Create Raffle | Stwórz Loterię, Stworz Loterie | true or false if you want users to join raffle by themselfs, string representing reward | Will create raffle for specified reward | $Create Raffle true rally awesome reward |
| Add Tickets To User | Dodaj Bilety Do Użytkownika, Dodaj Bilety Do Uzytkownika | Raffle id, user object and ticket amount | Will add specified amount of tickets to user for specified raffle | $Add Tickets To User 1 @Jurij98 100 |
| Add Tickets To Role | Dodaj Bilety Do Roli | Raffle id, role object and ticket amount | Will add specified amount of tickets to all users with role for specified raffle | $Add Tickets To Role 1 @ExampleRole 100 |
| Roll Raffle | Losuj | Raffle id | Will roll raffle winner | $Roll Raffle 1 |
| Slowmode | --- | Integer representing slowmode interval in seconds and list at least one channel object | Will add slowmode with specified interval to channels | %slowmode 1 #🥂general-discussion🥂 #😂memes😂 #🛰bot-channel🛰 |
| Give Role | Daj Rolę, Daj Role | Role object and optional user ping | Will add role to user | $give role @exampleRole @Jurij98 |
| Remove Role | Usuń Rolę, Usun Role | Role object and optional user ping | Will remove role from user | $remove role @exampleRole @Jurij98 |

[Back to top][BackToTop]

### User commands
Bolded commands are **special commands** available only for members of our [Discord server][DiscordInvite].
**Special commands** will be available only in English language.

| Command | Aliases | Parameters | Outcome | Example |
| --- | --- | --- | --- | --- |
| Status | --- | Optional user object | Will return basic information about user warns, praises and achievements | $status |
| Warns | Ostrzeżenia, Ostrzezenia | Optional user object | Will return more detailed information about user warns | $warns @Jurij98 |
| Praises | Pochwały, Pochwaly | Optional user object | Will return more detailed information about user praises | $praises |
| Achievements | Osiągnięcia, Osiagniecia | Optional user object | Will return more detailed information about user achievements | $achievements |
| Socials | Social Media | --- | Will return Rachel's socials | $socials |
| Credits | Twórcy, Tworcy | --- | Will return Rachel's credits | $credits |
| Help | Pomoc | --- | Will show basic information about how to get support | $help |
| Show Level Roles | Pokaż Role Za Level, Pokaz Role Za Level | --- | Will show all role rewards | $show level roles |
| Vote | Głosowanie, Glosowanie, Propozycja | String representing vote content | Will add few reactions under message to simulate voting system | $Vote something to vote |
| Poll | Ankieta | String representing poll question and possible answers. Poll question and each answer divided with \| character | Will create message with poll | $Poll Cats or Dogs?\|Cats\|Dogs |
| Leaderboard | Ranking | Optional integer representing how many users need to be in leaderboard and optional character representing type of leaderboard (a, p or x) | Will show server leaderboard | $Leaderboard 15 x |
| Quote | Zacytuj, Cytuj | Non-negative integer representing message id and optional text channel object | Will quote user message | $Quote 744688869567627264 #channel |
| Avatar | Awatar | Optional user object | Will show user avatar | $Avatar @Jurij98 |
| TicTacToe | --- | Optional user object | Will create TicTacToe game | $TicTacToe @Jurij98 |
| Join Raffle | Dołącz Do Loterii, Dolacz Do Loterii | Raffle id | Will try to join raffle with one ticket | $Join Raffle 1 |
| Show Raffle | Loteria | Raffle id | Will show raffle informations | $Show Raffle 1 |
| Show Raffles | Loterie | --- | Will show raffle list | $Show Raffles |
| Support Us | Wesprzyj Nas | --- | Will show message with ways to support project | $Support Us |
| **Any thoughts** | --- | --- | Will show one of Rachel responses | $Any thoughts |
| **CYOA List** | --- | --- | Will show list of available adventures | $CYOA List |
| **CYOA Profile** | --- | --- | Will show information about current user's adventure and MC | $CYOA Profile |
| **CYOA Change Profile** | --- | Boolean representing MC gender (true - if you want to play as female character, false - as male character) and string representing MC name | Will change MC gender and name | $CYOA Change Profile true Rachel |
| **CYOA Choose Adventure** | --- | String representing adventure code | Will change/reset current adventure | $CYOA Choose Adventure 5_Rachel_Origins_Jurij |
| **CYOA Play** | --- | --- | Will show current page of an adventure | $CYOA Play |
| **CYOA Make Choice** | --- | Integer representing choice id | Will progress the story | $CYOA Make Choice 1 |

[Back to top][BackToTop]

***
## Currently supported languages
- English
- Polski (Polish) *except special commands*

[Back to top][BackToTop]

***
## Support us
If you like Rachel bot consider giving us a GitHub star, joining our [Discord server][DiscordInvite] 
and becoming our [Patreon][PatreonPage].
![Affectionate Rachel by aster_atheris][RachelAffectionate]

***
## Credits
Idea and realization: Jerzy Kruszewski

Avatars: aster_atheris ([artist instagram page][ArtistInstagram])

[Back to top][BackToTop]

***
## License
Code: MIT License

Images: All rights reserved. They are intellectual properties of respective artist. 

[Back to top][BackToTop]

***
## Changelog
- Version 1.10.0
	- Added CYOA Player with 2 stories
- Version 1.9.0
	- Added simple dialogue system
	- Added channel slowmode manipulation
	- Added give and remove role commands
	- Fixed TicTacToe winner message
- Version 1.8.2
	- Fixed bug with user join/left messages
- Version 1.8.1
	- Various bugfixes
- Version 1.8.0
	- Added moderation announcements
	- Added raffles
	- Added support us command
- Version 1.7.1
	- Fixed typo
- Version 1.7.0
	- Added TicTacToe command
- Version 1.6.1
	- Removed unnecessary permission requirements
- Version 1.6.0
	- Added Polish language
	- Fixed bug with leaderboard
- Version 1.5.2
	- Added missing permission
- Version 1.5.1
	- Fixed bug with warning system
- Version 1.5.0
	- Added Remove Praise command
	- Added Remove Achievement command
	- Added Poll command
- Version 1.4.1
	- Removed unnecessary permission requirements
- Version 1.4.0
	- Added Reprimand command
	- Added Vote command
	- Added Leaderboard command
	- Added Quote command
	- Added Avatar command
- Version 1.3.0
	- Achievements system overhaul
- Version 1.2.0
	- Actually fixed critical bug
	- Improved achievement system code
	- Fixed some typos
	- Made Show Level Roles command available for everyone
- Version 1.1.0
	- Added bot owner commands
	- Added file logger
	- Administrators are now staff members by default
	- Fixed critical bug
- Version 1.0.1
	- Fixed issue with warns and praises ids
- Version 1.0.0
	- Initial release
	
[Back to top][BackToTop]

***
## What I have learned working on this project
- How to utilize power of Dependency Injections

[Back to top][BackToTop]

***
## Information for contributors
Rachel uses external libraries (found in Assemblies folder):
- Discord.Addons.Labs.Interactive which is just port of [foxbot's][FoxBotGitHub] [Discord.Addons.Interactive][DiscAddIntActRepo]
to Discord.Net.Labs.

[Back to top][BackToTop]

[BackToTop]: https://github.com/JerzyKruszewski/RachelDiscordBot#table-of-contents
[FeatureList]: https://github.com/JerzyKruszewski/RachelDiscordBot#feature-list
[BansAndKicks]: https://github.com/JerzyKruszewski/RachelDiscordBot#banning-and-kicking-users
[WarningSystems]: https://github.com/JerzyKruszewski/RachelDiscordBot#two-warning-systems-quantity--and-point-based
[PraiseSystem]: https://github.com/JerzyKruszewski/RachelDiscordBot#praise-system
[AchievementSystem]: https://github.com/JerzyKruszewski/RachelDiscordBot#point-based-xbox-like--achievement-system
[CustomizableCommandPrefix]: https://github.com/JerzyKruszewski/RachelDiscordBot#customizable-guild-server-wise-command-prefix-default-
[UserJoinLeftMessage]: https://github.com/JerzyKruszewski/RachelDiscordBot#customizable-guild-server-wise-user-join-and-left-message
[LevelingSystem]: https://github.com/JerzyKruszewski/RachelDiscordBot#simple-user-leveling-system
[ModerationAnnouncement]: https://github.com/JerzyKruszewski/RachelDiscordBot#moderation-announcement-system
[RaffleSystem]: https://github.com/JerzyKruszewski/RachelDiscordBot#raffle-system
[CYOAPlayer]: https://github.com/JerzyKruszewski/RachelDiscordBot#cyoa-player
[Requirements]: https://github.com/JerzyKruszewski/RachelDiscordBot#requirements
[AdditionalRequirements]: https://github.com/JerzyKruszewski/RachelDiscordBot#additional-requirements
[PermissionsUsage]: https://github.com/JerzyKruszewski/RachelDiscordBot#why-rachel-need-these-permissions-and-how-she-use-them
[GettingStarted]: https://github.com/JerzyKruszewski/RachelDiscordBot#getting-started
[BasicConfiguration]: https://github.com/JerzyKruszewski/RachelDiscordBot#basic-configuration
[CommandList]: https://github.com/JerzyKruszewski/RachelDiscordBot#command-list
[ConfigurationCommands]: https://github.com/JerzyKruszewski/RachelDiscordBot#configuration-commands
[ModeratorCommands]: https://github.com/JerzyKruszewski/RachelDiscordBot#moderator-commands
[UserCommands]: https://github.com/JerzyKruszewski/RachelDiscordBot#user-commands
[SupportedLanguages]: https://github.com/JerzyKruszewski/RachelDiscordBot#currently-supported-languages
[SupportUs]: https://github.com/JerzyKruszewski/RachelDiscordBot#support-us
[Credits]: https://github.com/JerzyKruszewski/RachelDiscordBot#credits
[License]: https://github.com/JerzyKruszewski/RachelDiscordBot#license
[Changelog]: https://github.com/JerzyKruszewski/RachelDiscordBot#changelog
[WhatIHaveLearned]: https://github.com/JerzyKruszewski/RachelDiscordBot#what-i-have-learned-working-on-this-project
[InfoForContributors]: https://github.com/JerzyKruszewski/RachelDiscordBot#information-for-contributors

[CodeFactorBadge]: https://www.codefactor.io/repository/github/jerzykruszewski/racheldiscordbot/badge
[CodeFactorRepository]: https://www.codefactor.io/repository/github/jerzykruszewski/racheldiscordbot
[DiscordIcon]: https://img.shields.io/discord/591914197219016707.svg?color=7289da&label=BajarzDevelopment&logo=discord&style=flat-square
[DiscordInvite]: https://discord.gg/TjCDEQU

[InviteRachelAdmin]: https://discord.com/api/oauth2/authorize?client_id=810093575500726302&permissions=8&scope=bot
[InviteRachelBasic]: https://discord.com/api/oauth2/authorize?client_id=810093575500726302&permissions=268454998&scope=bot
[InviteRachelPermless]: https://discord.com/api/oauth2/authorize?client_id=810093575500726302&permissions=0&scope=bot

[RachelNormal]: ./RachelBot/Images/normal.png
[RachelSmiling]: ./RachelBot/Images/smiling.png
[RachelAffectionate]: ./RachelBot/Images/affectionate.png

[ISOCodes]: https://en.wikipedia.org/wiki/List_of_ISO_3166_country_codes
[CYOATool]: https://bitbucket.org/JurijK/chooseyourownadventure/downloads/
[ArtistInstagram]: https://www.instagram.com/aster_atheris/
[PatreonPage]: https://www.patreon.com/bajarzdevelopment?fan_landing=true
[FoxBotGitHub]: https://github.com/foxbot
[DiscAddIntActRepo]: https://github.com/foxbot/Discord.Addons.Interactive
