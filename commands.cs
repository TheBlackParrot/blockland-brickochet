$Take::Root = "Add-Ons/Gamemode_Brickochet";

function serverCmdCancel(%this) {
	if(!isObject(%this.projectile)) {
		%this.play2D(errorSound);
		messageClient(%this,'',"You do not have a trail going!");
		return;
	}
	if(getSimTime() - %this.lastManualExplode < 3000) {
		%this.play2D(errorSound);
		messageClient(%this,'',"You can only cancel your trail every 3 seconds.");
		return;
	}
	
	%this.projectile.explode();
}

//function serverCmdChangeSymbol(%this,%which) {
//	%symbols = "149 162 163 165 128 36 164 167 169 174 153 182 181 223 230 248 215 134";
//	if(%which $= "" || %which > getWordCount(%symbols) || %which < 1) {
//		for(%i=0;%i<getWordCount(%symbols);%i++) {
//			messageClient(%this,'',"\c3" @ %i+1 @ "\c6." SPC chr(getWord(%symbols,%i)));
//		}
//		return;
//	}
//
//	%color = "<color:" @ RGBToHex(getColorIDTable(%this.color)) @ ">";
//
//	%this.clanSymbol = chr(getWord(%symbols,%which-1));
//	if(%this.originalPrefix $= "") {
//		if(%this.clanPrefix $= "") {
//			%this.originalPrefix = "%NULL%";
//		} else {
//			%this.originalPrefix = %this.clanPrefix;
//		}
//	}
//}

//http://forum.blockland.us/index.php?topic=272459.msg8083312#msg8083312
function chr(%i) {
	return $_byte_map[%i];
}
function ord(%c) {
	return 1 + strpos($_byte_list, %c);
}

if($_byte_list $= ""){
	for($_i=1;$_i<256;$_i++) {
		$_byte_map[$_i] = collapseEscape("\\x" @
			getSubStr("0123456789abcdef", $_i >> 4, 1) @
			getSubStr("0123456789abcdef", $_i & 15, 1));
		$_byte_list = $_byte_list @ $_byte_map[$_i];
	}
}

function serverCmdChangeColor(%this) {
	if(!isObject(%this.player)) {
		messageClient(%client,'',"You must be in the game to change your color.");
		return;
	}
	if(getSimTime() - %this.lastColorChange < 30000) {
		messageClient(%client,'',"You can only change your color every 30 seconds.");
		return;
	}

	%color = %this.player.currSprayCan;
	if(%color < 1 || %color > 63) {
		%color = 1;
	}

	for(%i=0;%i<$DefaultMinigame.numMembers;%i++) {
		%client = $DefaultMinigame.member[%i];
		// adding C makes things a lot easier later
		if(%color == %client.color) {
			messageClient(%this,'',"This color is already taken.");
			return;
		}
	}
	%this.color = %color;
	%this.player.setShapeNamecolor(getColorIDTable(%color));
	%this.lastColorChange = getSimTime();

	for(%i=0;%i<BrickGroup_888888.getCount();%i++) {
		%brick = BrickGroup_888888.getObject(%i);
		if(%brick.takenBy == %this) {
			%brick.setColor(%color);
		}
	}

	serverCmdChangeSymbol(%this,1);
}

function serverCmdHelp(%this) {
	if(getSimTime() - %this.lastCommandCalledAt < 500) {
		return;
	}
	%this.lastCommandCalledAt = getSimTime();

	%file = new FileObject();
	%file.openForRead($Take::Root @ "/help.txt");

	while(!%file.isEOF()) {
		//messageClient(%this,'',strReplace(%file.readLine(),"%%VERSION",$Mining::Version));
		messageClient(%this,'',%file.readLine());
	}

	%file.close();
	%file.delete();
}

function serverCmdLeaderboard(%this) {
	if(getSimTime() - %this.lastCommandCalledAt < 500) {
		return;
	}
	%this.lastCommandCalledAt = getSimTime();

	for(%i=0;%i<$DefaultMinigame.numMembers;%i++) {
		%client = $DefaultMinigame.member[%i];
		// clever code
		%rank_v = %client.getRank();
		%rank[%rank_v] = %client;
	}
	messageClient(%this,'',"-- CURRENT STANDINGS --");
	for(%i=1;%i<=$DefaultMinigame.numMembers;%i++) {
		messageClient(%this,'',"<just:left>\c2#" @ %i @ ".\c6" SPC %rank[%i].name @ "\c7 -- \c3" @ %rank[%i].amountHas SPC "\c7[" @ %rank[%i].getPercentageValue("players") @ "%]");
	}
}
function serverCmdLB(%this) {serverCmdLeaderboard(%this);}