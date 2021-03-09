# Rachel Discord Bot

[![CodeFactor][CodeFactorBadge]][CodeFactorRepository] [![Support Server][DiscordIcon]][DiscordInvite]
***
![Rachel by aster_atheris][RachelNormal]

## Feature list
- Banning and kicking users
- Two warning systems (quantity- and point-based)
- Praise system
- Point-based, XBOX-like  achievement system
- Mention command prefix
- Customizable guild (server) wise command prefix (default $)
- Customizable guild (server) wise user join and left message
- Simple user leveling system

***
## Requirements
There are no hard requirements but some commands won't work properly without additional permissions.

Additional "requirements":
- Rachel should have administrator privileges on your server.
	- If you don't want to give Rachel administrator privileges, you can give her:
		- Banning members
		- Kicking members
		- Managing roles
		- Managing channels
- Your server should have rules channel.

***
## Getting Started

### Basic Configuration
1. Make sure your server meets Rachel requirements.
2. [Invite Rachel.][InviteRachel] (or if you don't want to give her administrator privileges: [Use this link][NotRecommendedInviteRachelLink])
3. Make sure Rachel role is as high as possible in role hierarchy.
4. Call 4 commands:
	- $ChangeToSChannel **Required**
	- $ChangeModerationChannel *Optional*
	- $ChangeUsersJoiningChannel *Optional*
	- $ChangeUsersLeftChannel *Optional*
5. Enjoy!

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
| AddStaffRoles | at least one SocketRole object (discord role) | Adds roles to staff roles | $AddStaffRoles @Owner @Admin @Moderator |
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

### Moderator commands
**All moderation commands require user to have one of staff roles and some Rachel to have additional privileges (Banning members, kicking members, managing roles and channels).**

| Command | Parameters | Outcome | Example |
| --- | --- | --- | --- |
| Ban | User object and string representing ban reason | Will ban user | $ban @Jurij98 ban reason |
| Kick | User object and string representing kick reason | Will kick user | $kick @Jurij98 kick reason |
| Praise | User object and string representing praise reason | Will add praise to user account | $praise @Jurij98 praise reason |
| Reprimand | User object and string representing reprimand reason | Will reprimand user | $Reprimand @Jurij98 reason |
| Warn | User object and string representing warn reason | Will add warn to user account. If you use point-based warning system reason should start with non-negative integer | $warn @Jurij98 10 warn reason |
| Remove Warn | User object and integer representing warn id | Remove warn with specified id from user | $remove warn @Jurij98 23 |
| Achievement | User object, integer representing value of achievement and string representing achievement | Will add achievement to user account | $achievement @Jurij98 20 achievement |
| Unlock Channel | Role and at least one channel objects | Will unlock channel for role with default permissions | $unlock channel @Role #channel #other-channel |
| Lock Channel | Role and at least one channel objects | Will lock channel for role with default permissions | $lock channel @Role #channel #other-channel |
| Add Level Role | Role and non-negative integer representing level requirement | Will add role as leveling reward | $add level role @100lvlRole 100 |
| Remove Level Role | Role object |  Will remove role from leveling rewards | $remove level role @100lvlRole |

### User commands

| Command | Parameters | Outcome | Example |
| --- | --- | --- | --- |
| Status | Optional user object | Will return basic information about user warns, praises and achievements | $status |
| Warns | Optional user object | Will return more detailed information about user warns | $warns @Jurij98 |
| Praises | Optional user object | Will return more detailed information about user praises | $praises |
| Achievements | Optional user object | Will return more detailed information about user achievements | $achievements |
| Socials | --- | Will return Rachel's socials | $socials |
| Credits | --- | Will return Rachel's credits | $credits |
| Help | --- | Will show basic information about how to get support | $help |
| Show Level Roles | --- | Will show all role rewards | $show level roles |
| Vote | String representing vote content | Will add few reactions under message to simulate voting system | $Vote something to vote |
| Poll | String representing poll question and possible answers. Poll question and each answer divided with \| character | Will create message with poll | $Poll Cats or Dogs?\|Cats\|Dogs |
| Leaderboard | Optional integer representing how many users need to be in leaderboard and optional character representing type of leaderboard (a, p or x) | Will show server leaderboard | $Leaderboard 15 x |
| Quote | Non-negative integer representing message id and optional text channel object | Will quote user message | $Quote 744688869567627264 #channel |
| Avatar | Optional user object | Will show user avatar | $Avatar @Jurij98 |

***
## User join/leave messages placeholders
- {0} - user mention
- {1} - user username
- {2} - user id
- {3} - guild (server) name

***
## Currently supported languages
- English

***
## Support us
If you like Rachel bot consider giving us a GitHub star and we invite you to our [Discord server][DiscordInvite].
![Affectionate Rachel by aster_atheris][RachelAffectionate]

***
## Credits
Idea and realization: Jerzy Kruszewski

Avatars: aster_atheris ([artist instagram page][ArtistInstagram])

***
## License
Code: MIT License

Images: All rights reserved. They are intellectual properties of respective artist. 

***
## Changelog
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

***
## What I have learned working on this project
- How to utilize power of Dependency Injections

[CodeFactorBadge]: https://www.codefactor.io/repository/github/jerzykruszewski/racheldiscordbot/badge
[CodeFactorRepository]: https://www.codefactor.io/repository/github/jerzykruszewski/racheldiscordbot
[DiscordIcon]: https://img.shields.io/discord/591914197219016707.svg?color=7289da&label=BajarzDevelopment&logo=discord&style=flat-square
[DiscordInvite]: https://discord.gg/TjCDEQU
[InviteRachel]: https://discord.com/api/oauth2/authorize?client_id=810093575500726302&permissions=8&scope=bot
[NotRecommendedInviteRachelLink]: https://discord.com/api/oauth2/authorize?client_id=810093575500726302&permissions=268553238&scope=bot
[RachelNormal]: ./RachelBot/Images/normal.png
[RachelSmiling]: ./RachelBot/Images/smiling.png
[RachelAffectionate]: ./RachelBot/Images/affectionate.png
[ISOCodes]: https://en.wikipedia.org/wiki/List_of_ISO_3166_country_codes
[ArtistInstagram]: https://www.instagram.com/aster_atheris/
