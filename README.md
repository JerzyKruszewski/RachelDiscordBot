# Rachel Discord Bot

[![CodeFactor][CodeFactorBadge]][CodeFactorRepository] [![Support Server][DiscordIcon]][DiscordInvite]
***
![Rachel by aster_atheris][RachelNormal]

## Table of Contents
- [Feature list][FeatureList]
- [Requirements][Requirements]
	- [Additional "requirements"][AdditionalRequirements]
	- [Why Rachel need these permissions and how she use them][PermissionsUsage]
- [Getting Started][GettingStarted]
	- [Basic Configuration][BasicConfiguration]
- [Command List][CommandList]
	- [Configuration Commands][ConfigurationCommands]
	- [Moderator commands][ModeratorCommands]
	- [User commands][UserCommands]
- [User join/leave messages placeholders][UserJoinLeavePlaceholders]
- [Currently supported languages][SupportedLanguages]
- [Support us][SupportUs]
- [Credits][Credits]
- [License][License]
- [Changelog][Changelog]
- [What I have learned working on this project][WhatIHaveLearned]

***
## Feature list
- Banning and kicking users
- Two warning systems (quantity- and point-based)
- Praise system
- Point-based, XBOX-like  achievement system
- Mention command prefix
- Customizable guild (server) wise command prefix (default $)
- Customizable guild (server) wise user join and left message
- Simple user leveling system

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
- Manage channels
	- **$unlock channel** command won't work because Rachel will modify role permissions to channels
	- **$lock channel** command won't work because Rachel will modify role permissions to channels
- Manage messages
	- **$poll** command will work, but Rachel won't remove user message with poll content
	
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
| Warn | Ostrzeżenie, Ostrzezenia | User object and string representing warn reason | Will add warn to user account. If you use point-based warning system reason should start with non-negative integer | $warn @Jurij98 10 warn reason |
| Remove Warn | Usuń Ostrzeżenie, Usun Ostrzezenie | User object and integer representing warn id | Remove warn with specified id from user | $remove warn @Jurij98 23 |
| Achievement | Osiągnięcie, Osiagniecie | User object, integer representing value of achievement and string representing achievement | Will add achievement to user account | $achievement @Jurij98 20 achievement |
| Remove Achievement | Usuń Osiągnięcie, Usun Osiagniecie | User object and integer representing achievement id | Will remove achievement from user account | $remove achievement @Jurij98 1 |
| Unlock Channel | Odblokuj Kanał, Odblokuj Kanal | Role and at least one channel objects | Will unlock channel for role with default permissions | $unlock channel @Role #channel #other-channel |
| Lock Channel | Zablokuj Kanał, Zablokuj Kanal | Role and at least one channel objects | Will lock channel for role with default permissions | $lock channel @Role #channel #other-channel |
| Add Level Role | Dodaj Rolę Za Level, Dodaj Role Za Level | Role and non-negative integer representing level requirement | Will add role as leveling reward | $add level role @100lvlRole 100 |
| Remove Level Role | Usuń Rolę Za Level, Usun Role Za Level | Role object |  Will remove role from leveling rewards | $remove level role @100lvlRole |

[Back to top][BackToTop]

### User commands

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

[Back to top][BackToTop]

***
## User join/leave messages placeholders
- {0} - user mention
- {1} - user username
- {2} - user id
- {3} - guild (server) name

[Back to top][BackToTop]

***
## Currently supported languages
- English
- Polski (Polish)

[Back to top][BackToTop]

***
## Support us
If you like Rachel bot consider giving us a GitHub star and we invite you to our [Discord server][DiscordInvite].
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

[BackToTop]: https://github.com/JerzyKruszewski/RachelDiscordBot#table-of-contents
[FeatureList]: https://github.com/JerzyKruszewski/RachelDiscordBot#feature-list
[Requirements]: https://github.com/JerzyKruszewski/RachelDiscordBot#requirements
[AdditionalRequirements]: https://github.com/JerzyKruszewski/RachelDiscordBot#additional-requirements
[PermissionsUsage]: https://github.com/JerzyKruszewski/RachelDiscordBot#why-rachel-need-these-permissions-and-how-she-use-them
[GettingStarted]: https://github.com/JerzyKruszewski/RachelDiscordBot#getting-started
[BasicConfiguration]: https://github.com/JerzyKruszewski/RachelDiscordBot#basic-configuration
[CommandList]: https://github.com/JerzyKruszewski/RachelDiscordBot#command-list
[ConfigurationCommands]: https://github.com/JerzyKruszewski/RachelDiscordBot#configuration-commands
[ModeratorCommands]: https://github.com/JerzyKruszewski/RachelDiscordBot#moderator-commands
[UserCommands]: https://github.com/JerzyKruszewski/RachelDiscordBot#user-commands
[UserJoinLeavePlaceholders]: https://github.com/JerzyKruszewski/RachelDiscordBot#user-joinleave-messages-placeholders
[SupportedLanguages]: https://github.com/JerzyKruszewski/RachelDiscordBot#currently-supported-languages
[SupportUs]: https://github.com/JerzyKruszewski/RachelDiscordBot#support-us
[Credits]: https://github.com/JerzyKruszewski/RachelDiscordBot#credits
[License]: https://github.com/JerzyKruszewski/RachelDiscordBot#license
[Changelog]: https://github.com/JerzyKruszewski/RachelDiscordBot#changelog
[WhatIHaveLearned]: https://github.com/JerzyKruszewski/RachelDiscordBot#what-i-have-learned-working-on-this-project

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
[ArtistInstagram]: https://www.instagram.com/aster_atheris/
