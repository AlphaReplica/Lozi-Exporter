var loziobject;
	
Lozi.load('examples/canyon/canyon_asset.js',function(obj)
{
	loziobject = obj.object;
	scene.add(loziobject);
	
	onLoaded();
},onProgress);