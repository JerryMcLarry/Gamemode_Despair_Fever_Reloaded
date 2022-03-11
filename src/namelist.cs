function getRandomGender()
{
	if (getRandom(1))
		return "male";
	else
		return "female";
}

function getRandomName(%gender)
{
	return sampleNameList("first-" @ %gender) SPC sampleNameList("last");
}

function sampleNameList(%nameList)
{
	return $Despair::NameListItem[%nameList, getRandom($Despair::NameListMax[%nameList])];
}

function loadNameList(%nameList)
{
	%file = new FileObject();
	%fileName = $Despair::Path @ "data/" @ %nameList @ ".txt";

	if (!%file.openForRead(%fileName))
	{
		error("ERROR: Failed to open '" @ %fileName @ "' for reading");
		%file.delete();
		return;
	}

	deleteVariables("$Despair::NameListItem" @ %nameList @ "_*");
	%max = -1;

	while (!%file.isEOF())
	{
		%line = %file.readLine();
		$Despair::NameListItem[%nameList, %max++] = getWord(%line, 0);
	}

	%file.close();
	%file.delete();

	$Despair::NameListMax[%nameList] = %max;
}

function loadAllNameLists()
{
	loadNameList("first-male");
	loadNameList("first-female");
	loadNameList("last");
}

if (!$Despair::LoadedNameLists)
{
	$Despair::LoadedNameLists = true;
	loadAllNameLists();
}
