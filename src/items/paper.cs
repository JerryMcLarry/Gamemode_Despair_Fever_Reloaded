datablock ItemData(PaperstackItem)
{
	shapeFile = $Despair::Path @ "res/shapes/items/Paperstack.dts";
	mass = 1;
	drag = 0.3;
	density = 0.2;
	elasticity = 0;
	friction = 1;
	doColorShift = true;
	colorShiftColor = "1 1 1 1";
	uiName = "Paper Stack";
	canDrop = true;
};

function PaperstackItem::onAdd(%this, %obj)
{
	parent::onAdd(%this, %obj);
	%obj.papers = 10;
}

function PaperstackItem::onPickUp(%this, %obj, %player)
{
	if(%player.addTool(PaperItem) == -1)
		return;
	%obj.papers--;
	if(%obj.papers <= 0)
	{
		%obj.delete();
		return;
	}
}

datablock ItemData(PaperItem)
{
	shapeFile = $Despair::Path @ "res/shapes/items/Paper.dts";
	image = PaperImage;
	mass = 0.5;
	drag = 7;
	density = 0;
	elasticity = 0;
	friction = 1;
	gravityMod = 0.5;
	doColorShift = true;
	colorShiftColor = "1 1 1 1";
	uiName = "Paper";
	canDrop = true;

	smallItem = true;

	itemPropsClass = "PaperProps";
	itemPropsAlways = true;

	customPickupAlways = true;
	customPickupMultiple = true;
};

function PaperProps::onAdd(%this)
{
	%this.name = "Paper";
	%this.contents = ""; //Text
}

datablock ShapeBaseImageData(PaperImage)
{
	item = PaperItem;
	shapeFile = $Despair::Path @ "res/shapes/items/Paper.dts";
	doColorShift = true;
	colorShiftColor = "1 1 1 1";
	offset = "-0.2 0.17 0";
	eyeOffset = "0 0.7 -0.2";
	eyeRotation = eulerToMatrix("90 0 0");
	rotation = eulerToMatrix( "90 25 -45" );

	armReady = true;
};

function PaperItem::onUse(%this, %obj, %slot)
{
	%obj.unMountImage(0);
	%obj.mountImage(%this.image, 0);
	fixArmReady(%obj);
}

function PaperImage::onMount(%this, %obj, %slot)
{
	%props = %obj.getItemProps();
	%text = "(It's blank)";
	if(%props.contents !$= "")
		%text = %props.contents;
	if (isObject(%obj.client))
		commandToClient(%obj.client, 'CenterPrint', "<font:cambria:24>\c3" @ %props.name @ "\n<color:FFFFFF>" @ %text);
}

function PaperImage::onUnMount(%this, %obj, %slot)
{
	%props = %obj.getItemProps();
	if (isObject(%obj.client))
		commandToClient(%obj.client, 'ClearCenterPrint');
}

function getPaperEvidence()
{
	// %count = GameCharacters.getCount();
	// %innoCount = -1;
	// for (%i = 0; %i < %count; %i++)
	// {
	// 	%char = GameCharacters.getObject(%i);
	// 	if(!%char.killer)
	// 		%inno[%innoCount++] = %char;
	// }
	// // prepare
	// for (%i = 0; %i < %count; %i++)
	// 	%a[%i] = %i;
	// // shuffle
	// while (%i--)
	// {
	// 	%j = getRandom(%i);
	// 	%x = %a[%i - 1];
	// 	%a[%i - 1] = %a[%j];
	// 	%a[%j] = %x;
	// }

	%character = GameCharacters.getObject(getRandom(0, GameCharacters.getCount() - 1));
	%trait = getField(%character.traitList, getRandom(0, getFieldCount(%character.traitList) - 1));
	if(%trait $= "Chekhov's Gunman")
		return "meem";
	%msg = getWord(%character.name, 0) @ " is " @ aOrAn(%trait) SPC %trait SPC " kind of person!";
	return %msg;
}

// function getPaperEvidence()
// {
// 	%count = GameCharacters.getCount();
// 	%innoCount = -1;
// 	for (%i = 0; %i < %count; %i++)
// 	{
// 		%char = GameCharacters.getObject(%i);
// 		if(!%char.killer)
// 			%inno[%innoCount++] = %char;
// 	}
// 	// prepare
// 	for (%i = 0; %i < %count; %i++)
// 		%a[%i] = %i;
// 	// shuffle
// 	while (%i--)
// 	{
// 		%j = getRandom(%i);
// 		%x = %a[%i - 1];
// 		%a[%i - 1] = %a[%j];
// 		%a[%j] = %x;
// 	}
// 	%r = getRandom(1, 3);
// 	switch (%r)
// 	{
// 		case 1:
// 			%msg = "Investigation into robbery suspect reveals them to be";
// 			%decals = "";
// 			for (%i = 0; %i < %innoCount && getWordCount(%decals) < 3; %i++)
// 			{
// 				%char = GameCharacters.getObject(%a[%i]);
// 				%decal = getField(%char.appearance, 2);
// 				if(%decal $= "")
// 					%decal = "blank";
// 				if(strpos(%decals, %decal) == -1)
// 				{
// 					%decals = setWord(%decals, getWordCount(%decals), %decal);
// 				}
// 			}
// 			%killer = $pickedKiller.character;
// 			%decal = getField(%killer.appearance, 2);
// 			if(strpos(%decals, %decal) == -1)
// 			{
// 				%decals = setWord(%decals, getRandom(1, getWordCount(%decals)) - 1, %decal);
// 			}
// 			for(%i = 0; %i < getWordCount(%decals); %i++)
// 			{
// 				%decal = getWord(%decals, %i);
// 				switch$ (%decal)
// 				{
// 					case "Mod-Suit":
// 						%a = "a wealthy progeny";
// 					case "Mod-Pilot":
// 						%a = "an aspiring aviator";
// 					case "Mod-Army":
// 						%a = "an US Army fanatic";
// 					case "Meme-Mongler":
// 						%a = "a dinosaur enthusiast";
// 					case "Medieval-YARLY":
// 						%a = "a fan of Night Owls";
// 					case "Medieval-Rider":
// 						%a = "a Riders fan";
// 					case "Medieval-ORLY":
// 						%a = "a fan of White Owls";
// 					case "Medieval-Lion":
// 						%a = "a Lions fan";
// 					case "Medieval-Eagle":
// 						%a = "an Eagles fan";
// 					case "Hoodie":
// 						%a = "wearing a hoodie";
// 					case "Alyx":
// 						%a = "a Block Mesa fan";
// 					default:
// 						%a = "a Plain Shirt";
// 				}
// 				%list = setField(%list, getFieldCount(%list), %a);
// 			}
// 			%list = naturalGrammarList(%list, "or");
// 			%msg = %msg SPC %list;

// 		case 2:
// 			%character = %inno[getRandom(0, %innoCount-1)];
// 			%a = getSubStr(getWord(%character.name, 0), 0, 1);
// 			%b = getSubStr(getWord(%character.name, 1), 0, 1);
// 			%rng = getRandom(0, 2);
// 			if(%rng == 1)
// 				%a = "#";
// 			if(%rng == 2)
// 				%b = "#";

// 			%msg = %a @ "." @ %b @ ". was falsely accused of murder!";

// 		case 3:
// 			%msg = "A number of people -";

// 			%pick[1] = %inno[getRandom(0, %innoCount-1)];
// 			%pick[2] = %inno[getRandom(0, %innoCount-1)];
// 			%pick[3] = %inno[getRandom(0, %innoCount-1)];

// 			%pick[getRandom(1, 3)] = $pickedKiller.character;
// 			%i = 0;
// 			while(%i++ <= 3)
// 			{
// 				%a = getSubStr(getWord(%pick[%i].name, 0), 0, 1);
// 				%b = getSubStr(getWord(%pick[%i].name, 1), 0, 1);
// 				%rng = getRandom(1, 2);
// 				if(%rng == 1)
// 					%a = "#";
// 				if(%rng == 2)
// 					%b = "#";
// 				%list = %list @ (%i == 1 ? "" : "	") @ %a @ "." @ %b @ ".";
// 			}
// 			%list = naturalGrammarList(%list);
// 			%msg = %msg @ %list @ "- have been considered as possible murder suspects! Drama on page 5.";
// 	}
// 	return %msg;
// }

function getPaperTrash()
{
	%high = -1;

	%choice[%high++] = "HONK HONK HONK HONK HONK HONK HONK HONK HONK HONK HONK HONK";
	%choice[%high++] = "(This seems to be a patent for \"joke tape\", a roll of scotch tape without any actual glue on it.)";
	%choice[%high++] = ""; //You can write on blank
	%choice[%high++] = "(It seems to be a stylized drawing of a video game character.)";
	%choice[%high++] = "Skeletons have invaded the USA, taking over the country and renaming it to USS, short for \"the United States of Skeletons\".";
	%choice[%high++] = "He doesnt love me... im not gona be with a fake bitch like him!!!";
	%choice[%high++] = "To do: to do! to do, to do, to do, to do, to dooooooo, dodododo";
	%choice[%high++] = "To do: cut someone";
	%choice[%high++] = "To do: find some source of wifi holy shit there isn't even cell service";
	%choice[%high++] = ":^)";
	%choice[%high++] = "die";
	%choice[%high++] = "i got killed in csgo and i am so fkin mad rn";

	return %choice[getRandom(%high)];
}

function getPaperTips()
{
	%high = -1;

	%choice[%high++] = "Attacking people leaves your hands bloody! Choking and causing accidents doesn't, however.";
	%choice[%high++] = "Spilled Coke on your shirt? Just throw a coat on - nobody will see your dirty shirt from under it!";
	%choice[%high++] = "Be sure to admire yourself in the mirror by clicking it! There may be something on your face that you can't otherwise see.";
	%choice[%high++] = "A shower will not only completely clean you off, but will leave you feeling fresh and good about yourself!";
	%choice[%high++] = "Find a spooky mask to pull off a scary prank! With the mask on, they won't know it's you!";
	%choice[%high++] = "If you see something suspicious, scream! If you scream you'll be heard much farther.";
	%choice[%high++] = "Stick with someone who reflects your values! Otherwise you'll be in a constant state of internal conflict.";
	%choice[%high++] = "Getting everyone's alibi in check is very important! However, you should also learn time of death!";
	%choice[%high++] = "Someone blocking the doorway? Rapidly and furiously jostling them will usually get them to move!";

	return %choice[getRandom(%high)];
}

function getGuestList(%i)
{
	%charCount = GameCharacters.getCount();
	%div = 1; //always 0
	if(%charCount > 5)
	{
		%div = 2;
		%list = "<just:left><tab:160,340>";
	}
	if(%charCount > 10)
	{
		%div = 3;
		%list = "<just:left><tab:0,250,460>";
	}
	for (%i = 0; %i < %charCount; %i++)
	{
		%character = GameCharacters.getObject(%i);
		%name = %character.name;
		%room = $roomNum[%character.room];
		%initials = getSubStr(getWord(%character.name, 0), 0, 1) @ "." @ getSubStr(getWord(%character.name, 1), 0, 1) @ ".";
		%list = %list @ (%i % %div ? "	" : "") @ "\c6" @ %initials @ "\c3:\c6" @ %room @ ((%i % %div) == %div-1 ? "\n" : "	");
	}
	return %list;
}