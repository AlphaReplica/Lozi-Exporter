var loziobject;
	
Lozi.load('Examples/canyon/canyon_asset.js',function(obj)
{
	loziobject = obj.object;
	scene.add(loziobject);
	
	onLoaded();
},onProgress);
