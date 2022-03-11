$DecalLimit = 300;
$DecalTimeout = ""; // 30000;

if ($DecalTableHead $= "")
	$DecalTableHead = 0;

if (!isObject(DecalGroup))
    new SimGroup(DecalGroup);

function spawnDecal(%data, %position, %normal, %size, %color, %angle, %decalCombine, %noUnclutterCheck)
{
	if (%size $= "") %size = 1;
	if (%angle $= "") %angle = getRandom() * 6.28319;

	if (!%noUnclutterCheck)
	{
		initContainerRadiusSearch(%position, 0.001, $TypeMasks::StaticShapeObjectType);

		while (%decal = containerSearchNext())
		{
			if (%decal.isDecal && !%decal.noUnclutter)
			{
				if (%decal.decalCombine $= %decalCombine
				&& %decalCombine !$= "")
					%size = getMin(3, %size + getWord(%decal.getScale(), 0) * 0.5);
				%decal.delete();
			}
		}
	}

	%decal = new StaticShape()
    {
		datablock = %data;
        isDecal = 1;
		decalCombine = %decalCombine;
	};

	DecalGroup.add(%decal);

    if (isObject($DecalTable[$DecalTableHead]))
        $DecalTable[$DecalTableHead].delete();

    $DecalTable[$DecalTableHead] = %decal;
    if ($DecalTableHead == $DecalLimit)
        $DecalTableHead = 0;
    else
        $DecalTableHead++;
	
	%matrix = vectorToMatrix(%normal);
	%matrix = MatrixMultiply("0 0 0 " @ %normal SPC %angle, %matrix);
	%matrix = MatrixMultiply(%position, %matrix);

	%decal.setTransform(%matrix);

	if (%size !$= "")
		%decal.setScale(%size SPC %size SPC %size);

	%decal.normal = %normal;

	if (%color !$= "")
		%decal.setNodeColor("ALL", %color);

	if ($DecalTimeout !$= "")
    {
		// %decal.schedule($Pref::Server::DecalTimeout - 1000, fadeOut);
		%decal.schedule($DecalTimeout, delete);
	}

	return %decal;
}

function spawnDecalFromRayCast(%data, %ray, %size)
{
	if (%ray $= 0)
		return 0;

	%pos = getWords(%ray, 1, 3);
	%vec = getWords(%ray, 4, 6);
	%pos = VectorAdd(%pos, VectorScale(%vec, 0.01));

	return spawnDecal(%data, %pos, %vec, %size);
}

function sprayDecalWithRayCast(%data, %a, %b, %mask, %avoid, %size)
{
	return spawnDecalFromRayCast(%data, containerRayCast(%a, %b, %mask, %avoid), %size);
}

// function vectorToAxis(%vector)
// {
// 	%y = mRadToDeg(mACos(getWord(%vector, 2) / vectorLen(%vector))) % 360;
// 	%z = mRadToDeg(mATan(getWord(%vector, 1), getWord(%vector, 0)));

// 	%euler = vectorScale(0 SPC %y SPC %z, 3.14159 / 180);
// 	return getWords(matrixCreateFromEuler(%euler), 3, 6);
// }

function vectorToMatrix(%vector)
{
	%y = mACos(getWord(%vector, 2) / vectorLen(%vector));
	%z = mATan(getWord(%vector, 1), getWord(%vector, 0));
	return MatrixCreateFromEuler("0 " @ %y SPC %z);
}

function clearDecals()
{
	if (isObject(DecalGroup)) 
		DecalGroup.deleteAll();
}

function clearBloodBySource(%source) //used for RDM blood-clearing mostly
{
	for(%i = 0; %i < DecalGroup.getCount(); %i++)
	{
		%decal = DecalGroup.getObject(%i);
		if(%decal.isBlood && %decal.source == %source)
			%decal.schedule(0, delete);
	}
}

function serverCmdClearDecals(%this)
{
	if (!%this.isAdmin)
    {
		// messageClient(%this, '', "\c3You are not allowed to use this command.");
		return;
	}
	%decals = DecalGroupNew.getCount();
	// messageAll('MsgClearBricks', "\c3" @ %this.getPlayerName() @ "\c2 cleared all decals on the server." SPC "(" @ %decals @ " decals)");
	clearDecals();
}