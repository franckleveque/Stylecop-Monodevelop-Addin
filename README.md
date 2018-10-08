# Stylecop-Monodevelop-Addin
Addin allowing the use of stylecop with monodevelop
This addin is first intended for my personal use as I really miss stylecop on my Linux computer.

# Short instruction on install

1. Retrieve last version of sources
1. Compile the obtained solution
   - Open the project in Monodevelop (MonoDevelop.StyleCop.sln)
   - Select in Project / Active configuration Release
   - Right click on the MonoDevelop.StyleCop project in the solution pad and click on build ...
1. Copy all the dll in the Release folder in your monodevelop addin directory.
   - Right click on the MonoDevelop.StyleCop project in the solution pad and click on Open folder in explorer
   - Copy the files of the bin/release folder in the addins folder of your monodevelop installation.

**Beware**
On Linux systems, you might need to run stylecopcmd or Monodevelop(https://github.com/inorton/StyleCopCmd) as root prior to use Stylecop mono addin as Stylecop needs to create a registry key on the first run.

This version was created against Monodevelop 2.4, futher version might encounter compatibility problems.

# Publication
Here you can have a look on my new book on Stylecop : [Instant Stylecop Code Analysis How-to](http://bit.ly/15hO2ni)
