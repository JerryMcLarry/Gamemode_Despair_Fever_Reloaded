//This is a script file for /help and /rules commands
function serverCmdHelp(%this, %cat)
{
	%cat = strLwr(%cat);
	switch$ (%cat)
	{
		case "1" or "gamemode":
			%text[%count++] = "\c3[GAMEMODE]";
			%text[%count++] = " \c6The first day is the day without the killer. However, as soon as night strikes, the culprit is picked.";
			%text[%count++] = " \c6Killer's objective is to murder someone and get away with it.";
			%text[%count++] = " \c6Once the killer does their job, they have to blend in and try to seem least suspicious.";
			%text[%count++] = " \c6A body discovery announcement is initated when at least two people examine/scream at the body.";
			%text[%count++] = " \c6After a body has been discovered, investigation period starts.";
			%text[%count++] = " \c6During investigation, the crime scene is \c3\"frozen\"\c6 - Nobody can modify the state of the scene and nobody will be able to use weapons.";
			%text[%count++] = " \c6During this period, you will gather \c3Evidence\c6 - \c3Fibers, Alibis, Murder Weapon, Murder Method\c6 - gather as much information as possible!!!";
			%text[%count++] = " \c6After investigation period ends, the trial period starts.";
			%text[%count++] = " \c6In trial, everyone is teleported to the courtroom where you are given time to discuss everyone's alibis and possible suspects.";
			%text[%count++] = " \c6After trial period is over, it's time to vote the most suspicious person.";
			%text[%count++] = " \c6If majority vote is correct and everyone votes the killer, the killer dies and everyone else lives.";
			%text[%count++] = " \c6However, if the vote is a tie, people didn't vote or majority vote is wrong, the killer wins and everyone else is executed.";
			%text[%count++] = " \c6Trial period is the climax of the round - make sure to pay close attention and figure out the culprit!";
			%text[%count++] = " \c5Page Up to read the above.";
		case "2" or "chat":
			%text[%count++] = "\c3[CHAT]";
			%text[%count++] = " \c6Normal chat is local/IC (In-Character) chat. Put @ before your message to whisper (like so: \c3\"@whisper\"\c6)";
			%text[%count++] = " \c6You can also say /me *action* to do an IC action, like \c3\"John Doe grabs a weapon.\"";
			%text[%count++] = " \c6To use Killer-to-Admin chat as the killer, use \c3TEAM CHAT\c6 (Default key: \c3\"Y\"\c6)";
			%text[%count++] = " \c6Please follow the chat etiquette defined in the /rules.";
			%text[%count++] = " \c5Page Up to read the above.";
		case "3" or "killer":
			%text[%count++] = "\c3[KILLER]";
			%text[%count++] = " \c6You can become the killer at first night or by killing the killer.";
			%text[%count++] = " \c6Once you become the killer, you have to kill someone and get away with it. Killing people in public or having a killing spree is completely discouraged.";
			%text[%count++] = " \c6You can only kill so much. After investigation starts, your weapon will be disabled after 30 seconds and you will be unable to kill directly.";
			%text[%count++] = " \c6You can clean up the blood with a mop. To wash blood off yourself, you can use sinks, showers, water pumps, buckets or anything of the like.";
			%text[%count++] = " \c6Sinks, showers, buckets, water pumps clean you head to toe.";
			%text[%count++] = " \c6To clean your weapon, take it out when washing and use \c3\"Paint Key\"\c6 to activate events.";
			%text[%count++] = " \c6You also have a number of cool abilities!";
			%text[%count++] = " \c6You can sprint with \c3\"JET\" \c6key (default\c3: \"RightClick\"\c6)";
			%text[%count++] = " \c6When holding a sleeping person, you can hold \c3\"JET\" \c6key to choke them out! This is a bloodless and silent way to kill someone.";
			%text[%count++] = " \c6You can hear nearby heartbeats with \c3\"LIGHT\" \c6key (default\c3: \"R\"\c6)";
			%text[%count++] = " \c6And, since you will \c3never get tired\c6, you can fake \c3/sleep\c6 and get up any time!";
			%text[%count++] = " \c6Being a killer is very hard. Make sure to plan, but don't take your time!";
			%text[%count++] = " \c5Page Up to read the above.";
		case "4" or "mechanics":
			%text[%count++] = "\c3[MECHANICS]";
			%text[%count++] = " \c6To unlock/lock the door with the key, press \c3\"FIRE\"\c6 and \c3\"JET\"\c6 keys (default\c3: \"LeftClick\", \"RightClick\"\c6) respectively.";
			%text[%count++] = " \c6You can sleep with /sleep if you're sleepy, tired or exhausted. It's highly reccomeneded to go to sleep early, otherwise you will have to sleep way more often!";
			%text[%count++] = " \c6Dormitory rooms are the \c3best place to sleep\c6. They are your little safe havens. Make sure to lock the door!";
			%text[%count++] = " \c6Press \c3\"LIGHT\"\c6 key (default\c3: \"R\"\c6) when aiming at a door to knock, and when aiming at the corpse to loot it.";
			%text[%count++] = " \c6To navigate the inventory menu, scroll w/ mouse and click to take something out. To plant something on the corpse, drop an item when looking at it.";
			%text[%count++] = " \c3\"Hold-click\" and move your mouse\c6 to carry around a corpse.";
			%text[%count++] = " \c6You can \c3Scream\c6 with \c3\"ALARM\"\c6 key (default\c3: \"H\"\c6). However, you must be looking at something screwed up, like blood or a body.";
			%text[%count++] = " \c6People leave \c3Fibers\c6 during fights, sleeping or dragging bodies! The fibers take on the colors of their hair or clothes. Coats and hair-hiding masks obscure fibers!";
			%text[%count++] = " \c6There are \c0NO NON-LETHAL WEAPONS\c6 in this gamemode. Everything will kill.";
			%text[%count++] = " \c5Page Up to read the above.";
		case "5" or "combat":
			%text[%count++] = "\c3[COMBAT]";
			%text[%count++] = " \c6You can find melee weapons all over the map.";
			%text[%count++] = " \c6EVERYTHING can \c3BREAK DOWN DOORS\c6, however, it takes a while to do so and it's incredibly loud, and effectiveness varies on the weapon.";
			%text[%count++] = " \c6Attacks from behind cause \c3Shock\c6. This causes the victim to be slow and sluggish. Use this to your advantage!";
			%text[%count++] = " \c6If you fall in combat, you will enter \c0Critical Health\c6. In that state, your speaking range will be reduced and you will not able to scream as much.";
			%text[%count++] = " \c0You cannot be saved\c6 from that state. However, you can leave your last message with \c3/w[rite] [msg]\c6! Be sure to be aiming at a surface.";
			%text[%count++] = " \c6There are plans to expand the combat system, so stay tuned for updates!";
			%text[%count++] = " \c5Page Up to read the above.";
		case "6" or "rules":
			%text[%count++] = "\c3[RULES]";
			%text[%count++] = " \c31\c6. \c0Play your role!\c6 Play to survive. Accomplices to murder are strictly forbidden. Do not intentionally and directly help the killer.";
			%text[%count++] = " \c32\c6. \c0Keep chat clean!\c6 Don't reference people by their BL usernames. Don't spam chat. Don't abuse emoticons.";
			%text[%count++] = "           \c6Don't say racial, disability or any other kinds of slurs.";
			%text[%count++] = " \c33\c6. \c0Don't freekill!\c6 You CANNOT attack people for any other reason than self-defence. Threatening people or warning them is \c0NOT AN EXCUSE.";
			%text[%count++] = "           \c6If they swing their weapon at you or are hiding in your closet with their weapon out it counts as SELF-DEFENCE.";
			%text[%count++] = " \c34\c6. \c0Don't metagame!\c6 Do not relay in-game information to others through out-of-game means! If we determine you are metagaming you will be banned.";
			%text[%count++] = " \c35\c6. \c0Don't ERP (Erotic RolePlay)!\c6 While romance and relationship are not forbidden, please refrain from acting out explicit details.";
			%text[%count++] = " \c36\c6. \c0Don't be a idiot.\c6 We're all here to have fun. If you block doorways, lock people in your rooms or break room doors as a non-killer w/o a reason, don't expect for us to help if you get RDM'ed.";
			%text[%count++] = " \c0    EXAMPLES PROVIDED IN THIS LIST ARE ONLY EXAMPLES. THEY DO NOT ENCAPSULATE THE FULL EXTENT OF THE RULE'S EFFECT.";
			%text[%count++] = " \c3If someone is breaking the rules, use /report *message* to get an admin's attention!";
			%text[%count++] = " \c5Page Up to read the above.";
		case "7" or "commands":
			%text[%count++] = "\c3[COMMANDS]";
			%text[%count++] = " \c6/keepchar \c7- \c6Enable or disable character persistance (if you survive a round, you keep your character for the next)";
			%text[%count++] = " \c6/forcevote \c7- \c6Skip the discussion phase and get straight to the voting (Trial)";
			%text[%count++] = " \c6/spectate \c7- \c6Become a spectator to be excluded from rounds";
			%text[%count++] = " \c6/stats \c7- \c6See your statistics!";
			%text[%count++] = " \c6/sleep \c7- \c6Sleep.";
			if (%this.isAdmin)
			{
				%text[%count++] = "\c3[ADMIN]";
				%text[%count++] = " \c6Team Chat \c7- \c6Talk through admin-only chat";
				%text[%count++] = " \c3@\c6 before Team Chat e.g. \c3@message \c7- \c6Talk to the killer directly (cannot see killer chat when alive)";
				%text[%count++] = " \c6/menu \c7- \c6Bring up the logs";
				%text[%count++] = " \c6/whoIs \c3name \c7- \c6Find out who \c3name\c6's in-game alias is";
				%text[%count++] = " \c6/showroles \c7- \c6See who's killer or innocent when spectating";
				%text[%count++] = " \c6/announce \c3message \c7- \c6Send a server-wide announcement";
				%text[%count++] = " \c6/banlogs \c3bl_id OR name \c7- \c6See someone's punishment history";
				%text[%count++] = " \c6/warn \c3name warning \c7- \c6Warn someone";
				%text[%count++] = " \c6/kill \c3name \c7- \c6Forcekill someone";
			}
			%text[%count++] = " \c5Page Up to read the above.";
		default:
			%text[%count++] = "<font:impact:30>\c6Welcome to \c5Despair Fever\c6!";
			%text[%count++] = "\c6======";
			%text[%count++] = " \c6Available topics:";
			%text[%count++] = "   \c31\c6 - \c3gamemode\c6: How the gamemode progresses";
			%text[%count++] = "   \c32\c6 - \c3chat\c6: Chat functionality explained";
			%text[%count++] = "   \c33\c6 - \c3killer\c6: What to do if you're killer";
			%text[%count++] = "   \c34\c6 - \c3mechanics\c6: Game mechanics - locking doors, papers, sleeping, etc.";
			%text[%count++] = "   \c35\c6 - \c3combat\c6: Health and Melee explained";
			%text[%count++] = "   \c36\c6 - \c3rules\c6: Read this to avoid getting banned";
			%text[%count++] = "   \c37\c6 - \c3commands\c6: Various slash commands";
			%text[%count++] = "\c6======";
			%text[%count++] = "\c5Say \c3/help *category*\c5 for more info on certain topics.";
			%text[%count++] = "\c5If you want to read the game regulations (e.g. game rules), use \c3/regulations\c5.";
			%text[%count++] = "\c5We also have a <a:https://despair-fever.fandom.com/wiki/Despair_Fever_Wiki>Wiki</a>\c5!";
	}

	for (%i=1; %i<=%count; %i++)
		messageClient(%this, '', %text[%i]);
}

function serverCmdRules(%this)
{
	serverCmdHelp(%this, "rules");
}

function serverCmdRegulations(%this, %cat)
{
	%text[%count++] = "\c3[KILLING GAME REGULATIONS]";
	%text[%count++] = " \c31\c6. The killer, later referred to as the \c7'Wolf'\c6, is a person to deal the final killing blow on their victim.\c6";
	%text[%count++] = " \c32\c6. Should the current \c7'Wolf'\c6 be murdered, the person to deal the Killing Blow will be the \c7'Wolf'\c6.\c6";
	%text[%count++] = " \c33\c6. Should the victim's death be caused indirectly, the \c7'Wolf'\c6 is the recent person to push or attack the victim, or the setter of the trap.";
	%text[%count++] = " \c34\c6. A Class Trial will be held when a body is discovered by \c3two\c6 or more people.\c6";
	%text[%count++] = " \c35\c6. In the Class Trial, the 'Sheep' will have to face against the single \c7'Wolf'\c6.\c6";
	%text[%count++] = " \c36\c6. The \c7'Wolf'\c6 is determined at the end of a Class Trial by a majority vote. In the event of a tie it's a \c7'Wolf'\c6 victory.\c6";
	%text[%count++] = " \c37\c6. Should only two people remain alive, they will be forced to engage in combat for the last man standing.\c6";
	%text[%count++] = " \c5Page Up to read the above.";

	for (%i=1; %i<=%count; %i++)
		messageClient(%this, '', %text[%i]);
}
