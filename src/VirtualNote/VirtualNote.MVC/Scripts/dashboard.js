var Dashboard = function ()
{
	return { init: init };
	
	function init ()
	{		
		$('.datatable').dataTable ();
		$('.uniform').find ('input, select').uniform ();
		$('input, textarea').placeholder ();
	}	
}();