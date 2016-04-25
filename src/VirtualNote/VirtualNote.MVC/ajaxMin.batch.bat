
::::::::::::::::::::::: CSS :::::::::::::::::::::::::::::

:: Minimizar os main
ajaxmin -clobber Content\css\reset.css Content\css\text.css Content\css\form.css Content\css\buttons.css Content\css\grid.css Content\css\layout.css Content\css\mvc-css.css Content\css\custom.css -o Content\css\master-min.css


	
:: Minimizar UI-Darkness
ajaxmin -clobber Content\css\ui-darkness\jquery-ui-1.8.12.custom.css -o Content\css\ui-darkness\jquery-ui-1.8.12.custom-min.css
	
	
:: Minimizar os plugin
ajaxmin -clobber Content\css\plugin\jquery.visualize.css Content\css\plugin\uniform.default.css Content\css\plugin\dataTables.css -o Content\css\plugin\plugins-min.css





:::::::::::::::::::::::::: JS ::::::::::::::::::::::::::::::::: 

:: Minimizar os main

ajaxmin -clobber Scripts\jquery\jquery-1.5.2.min.js Scripts\jquery\jquery.extensions.js Scripts\jquery\jquery-ui-1.8.12.custom.min.js Scripts\jquery\jquery.visualize.js Scripts\jquery\jquery.dataTables.min.js Scripts\jquery\jquery.uniform.min.js Scripts\jquery\jquery.placeholder.min.js -o Scripts\jquery\master-min.js

pause



