/**
 * Created by Beka on 22.05.2016.
 */
 
function getProperties()
{
    var prop  = {};
    var query = window.location.search.substring(1);
    var vars = query.split("&");
    for (var num = 0; num < vars.length; num++)
    {
        var pair = vars[num].split("=");
        prop[pair[0]] = pair[1];
    }
    return prop;
}
var properties = getProperties();