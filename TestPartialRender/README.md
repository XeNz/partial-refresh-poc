# What?
- POC to test partial refreshing when debugging

##  How to test:
- Debug the project, this results in partial views being recompiled due to Microsoft.Extensions.DependencyInjection.RazorRuntimeCompilationMvcBuilderExtensions.AddRazorRuntimeCompilation
- This call in the Program.cs is intentionally put under a debug directive, as you do not want this in prod unless you really need it :') 
- Call the root endpoint @ http://localhost:5032/
  - You should get a button as html
    - ```html
      <button class="test">Test file</button>
        ```
  - Edit the button to have an extra class
    - reload your browser page
    - check that the extra class is on the button