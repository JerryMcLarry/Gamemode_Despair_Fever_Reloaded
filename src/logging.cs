//MedRP logging system

// Admin client handshake.
function serverCmdRPA_Handshake(%this, %version)
{
	%this.hasRPA = true;
	%this.RPAVersion = %version;
}

//Log function
function RS_Log(%line, %color, %superAdmin)
{
	if(%line $= "")
	{
		return;
	}

	%dateTime = getDateTime();
	%date = strreplace(firstWord(%dateTime), "/", "-");

	%fileName = "config/server/despairfever/logs/" @ %date @ ".txt";

	%file = new FileObject();
	%file.openForAppend(%fileName);

	%file.writeLine("[" @ %dateTime @ "] " @ %line);

	%file.close();
	%file.delete();

	// echo("[Logged]" SPC %line);

	for(%i = 0; %i < ClientGroup.getCount(); %i++)
	{
		%client = ClientGroup.getObject(%i);

		if(%client.isAdmin && (!%superAdmin || %client.isSuperAdmin))
		{
			commandToClient(%client, 'RP_Log', %color @ %line, %superAdmin);
		}
	}
}

// gui
function serverCmdMenu(%this)
{
	if(!%this.isAdmin)
		return;
	
	commandToClient(%this, 'RSAdmin_showMenu');
}

function serverCmdBanLogs(%this, %targetID)
{

	if(!%this.isAdmin || %targetID $= "")
		return;

	if(!(%targetID > 0))
		%targetID = findclientbyname(%targetID).getBLID();

	if(%targetID $= "")
		return;

	messageClient(%this, '', "<font:Palatino Linotype:28>\c4Punishment Logs for \c6 BL_ID " @ %targetID @ "\c4:");
	%this.play2D(RoleplayChatSound);
	
	if(!isFile("config/server/despairfever/logs/BanLog.txt"))
		return;
	%f = new fileObject();
	%f.openForRead("config/server/despairfever/logs/BanLog.txt");
	
	while(!%f.isEOF())
	{
		%line = %f.readLine();

		%ID = getField(%line, 0);
		if(%ID $= %targetID)
		{
			%date = getField(%line, 1);
			%type = getField(%line, 2);
			%time = getField(%line, 3);
			%reason = getField(%line, 4);

			if(%type $= "Ban")
				messageClient(%this, '', "\c4" @ %date @ "\c4 - " @"\c3[Ban]" @ "\c4 - " @ "\c6" @ %time @ " minutes" @ "\c4 - " @ "\c6" @ %reason);
			if(%type $= "Kick")
				messageClient(%this, '', "\c4" @ %date @ "\c4 - " @"\c3[Kick]");
			if(%type $= "Warning")
				messageClient(%this, '', "\c4" @ %date @ "\c4 - " @"\c3[Warning]" @ "\c4 - " @ "\c6" @ %reason);
		}
	}
	%f.close();
    %f.delete();
}
function serverCmdShowLogs(%this, %targetID)
{serverCmdBanLogs(%this, %targetID);}

function serverCmdLogs(%this, %targetID)
{serverCmdBanLogs(%this, %targetID);}

function serverCmdSL(%this, %targetID)
{serverCmdBanLogs(%this, %targetID);}

function serverCmdWarn(%this, %target, %a1, %a2, %a3, %a4, %a5, %a6, %a7, %a8, %a9, %a10, %a11, %a12, %a13, %a14, %a15, %a16, %a17, %a18, %a19, %a20, %a21, %a22, %a23, %a24, %a25, %a26, %a27, %a28, %a29, %a30, %a31, %a32)
{
	if(!%this.isAdmin)
		return;
	
	%target = findclientbyname(%target);
	
	if(!isObject(%target))
		return;
	
	%msg = trim(%a1 SPC %a2 SPC %a3 SPC %a4 SPC %a5 SPC %a6 SPC %a7 SPC %a8 SPC %a9 SPC %a10 SPC %a11 SPC %a12 SPC %a13 SPC %a14 SPC %a15 SPC %a16 SPC %a17 SPC %a18 SPC %a19 SPC %a20 SPC %a21 SPC %a22 SPC %a23 SPC %a24 SPC %a25 SPC %a26 SPC %a27 SPC %a28 SPC %a29 SPC %a30 SPC %a31 SPC %a32);
	%msg = StripMLControlChars(%msg);

	if(%msg $= "")
	{
		return;
	}
	
	messageClient(%this, '', '%1\c4You warned \c6%2\c4: \c6%3', "<bitmap:Add-Ons/Server_Roleplay/res/icons/" @ %this.getUserIcon() @ "> ", %target.getPlayerName(), %msg);
	%this.play2D(RoleplayChatSound);
	
	messageClient(%target, '', '%1\c4An admin has issued you a \c0WARNING\c4:', "<bitmap:Add-Ons/Server_Roleplay/res/icons/" @ %this.getUserIcon() @ "> ");
	messageClient(%target, '', '%1\c6%2', "<bitmap:Add-Ons/Server_Roleplay/res/icons/" @ %this.getUserIcon() @ "> ", %msg);
	%target.play2D(RoleplayChatSound);
	
	for(%i = 0; %i < ClientGroup.getCount(); %i++)
	{
		%client = ClientGroup.getObject(%i);

		if(!%client.isAdmin || %client == %this)
			continue;
		messageClient(%client, 'MsgAdminForce', '%1\c3%4 \c4warned \c6%2\c4: \c6%3', "<bitmap:Add-Ons/Server_Roleplay/res/icons/" @ %this.getUserIcon() @ "> ", %target.getPlayerName(), %msg, %this.getPlayerName());
	}
			
	RS_Log(%this.getPlayerName() SPC "(" @ %this.getBLID() @ ") warned " @ 
		 %target.getPlayerName() SPC "(" @ %target.getBLID() @ ") for reason: '" @ %msg @ "'");
	
	%victimID = %target.getBLID();
	
	// File I/O Operations
	%f=new fileObject();
	if(!isFile("config/server/despairfever/logs/BanLog.txt"))
	{
		%f.openForWrite("config/server/despairfever/logs/BanLog.txt");
		%f.writeLine("victimID" TAB "date" TAB "type" TAB "time" TAB "reason");
		%f.close();
	}
	%date = getDateTime();
	%f.openForAppend("config/server/despairfever/logs/BanLog.txt");
	%f.writeLine(%victimID TAB %date TAB "Warning" TAB "0" TAB %msg);
	%f.close();
	%f.delete();
}

function serverCmdDoBanCalc(%this, %curr, %prev)	// %curr = current offenses, %prev = previous offenses)
{
	if(!%this.isAdmin)
		return;
	
	if(%curr $= "")
		return;
	if(!(%curr > 0))
		return;
	if(%prev $= "")
		%prev = 0;
	
	%curr = mFloor(%curr);
	%prev = mFloor(%prev);
	
	%realhours = 6 * mPow(2, (%curr - 1)) * mPow(2, %prev);
	
	messageClient(%this, '', "\c3" @ %curr @ "\c4 Current RDM Offense" @ (%curr == 1 ? "" : "s") @ " \c6and \c3" @ %prev @ "\c4 Previous RDM Offense" @ (%prev == 1 ? "" : "s") @ "\c6:");
	if(%curr < 7 && %prev < 7)
	{
		%days = mFloor(%realhours / 24);
		%hours = %realhours - (%days * 24);
		%minutes = (%realhours - mfloor(%hours) - %days * 24) * 60;
		

		messageClient(%this, '', "\c2" @ %days @ " Day" @ (%days == 1 ? "" : "s") @ "; " @ %hours @ " Hour" @ (%hours == 1 ? "" : "s") @ "; " @ %minutes @ " Minute" @ (%minutes == 1 ? "" : "s"));
	}
	else
		messageClient(%this, '', "\c2Permanent. Bye bye!");
}

// PACKAGE

package RS_BanLogs
{
	function serverCmdUnBan(%client, %idx)
	{
		parent::serverCmdUnban(%client, %idx);
		if(!%client.isAdmin)
			return;
		%bannedID = banManagerSO.victimBL_ID[%idx];
		%bannedName = banManagerSO.victimName[%positionInList];
		
		RS_Log(%client.getPlayerName() SPC "(" @ %client.getBLID() @ ") unbanned BL_ID " @ %bannedID @ ".", "\c2", true);
	}
	function serverCmdBan(%client,%victim,%victimID,%time,%reason,%six,%seven,%eight)
	{
		if(!%client.isAdmin)
			return;
		%f=new fileObject();
		if(!isFile("config/server/despairfever/logs/BanLog.txt"))
		{
			%f.openForWrite("config/server/despairfever/logs/BanLog.txt");
			%f.writeLine("victimID" TAB "date" TAB "type" TAB "time" TAB "reason");
			%f.close();
		}
		if(%victim.inSit)
			serverCmdKickFromSit(%client, %victim);
		%date = getDateTime();
		%f.openForAppend("config/server/despairfever/logs/BanLog.txt");
		%f.writeLine(%victimID TAB %date TAB "Ban" TAB %time TAB %reason);
		%f.close();
		%f.delete();
		RS_Log(%client.getPlayerName() SPC "(" @ %client.getBLID() @ ") banned " @ 
			 %victim.getPlayerName() SPC "(" @ %victimID @ ") for reason: '" @ %reason @ "'");
		parent::serverCmdBan(%client,%victim,%victimID,%time,%reason,%six,%seven,%eight);
	}
	
	function serverCmdKick(%client,%victim)
	{
		%targ = %victim;
		if(!%client.isAdmin)
			return;
		%string = %victim;
		
		for(%i = 0; %i <= 9; %i++)
			%string = strReplace(%string, %i, "");
		
		if(strLen(%string) > 0)
			%numeric = false;
		else
			%numeric = true;
		

		
		if(!%numeric)
			%victim = findclientbyname(%victim);
		

		%f=new fileObject();
		if(!isFile("config/server/despairfever/logs/BanLog.txt"))
		{
			%f.openForWrite("config/server/despairfever/logs/BanLog.txt");
			%f.writeLine("victimID" TAB "date" TAB "type" TAB "time" TAB "reason");
			%f.close();
		}
		%f.openForAppend("config/server/despairfever/logs/BanLog.txt");
		%victimID = %victim.getBLID();
		%date = getDateTime();
		%f.writeLine(%victimID TAB %date TAB "Kick");
		%f.close();
		%f.delete();

		parent::serverCmdKick(%client,%victim);
	}
};

activatePackage(RS_BanLogs);
