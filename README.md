# spacecolony
Mobile Apps project. A 2D top-down space simulation colony game for mobile made in Unity.


## Get Started Developing
We are using Unity so that we can build for Android, Apple, PC, etc. all in one. For Android, some editing of the final APK must be done in Android Studio. Ask Mason about this.

1. Download the git client of your choice, such as [Git](https://git-scm.com/downloads), [Github Desktop](https://desktop.github.com/), [Sourcetree](https://www.sourcetreeapp.com/), etc.
2. Choose 'clone an existing repository' or equivalent and clone from the url https://github.com/MobileAppsMRHP/spacecolony.git
  - If you encounter authentication (403) errors in Sourcetree, try using username and password login instead of SSH login
3. You're set. 

## Using Git
- Before you start working, consider letting other people know in order to prevent overwriting file conflicts when you push your changes.
- It's good practice to always pull changes before you start working. Do so by pressing 'fetch origin' in Github Desktop or 'pull' in Sourcetree.

1. Open your git client and find the place where you look at your staged changes. In Github Desktop, this is the main screen. In Sourcetree, this is the Log / History tab at the bottom. 
2. (Sourcetree only) Select the files you want to commit via 'stage selected' and/or 'stage all'
3. Give your commit a message (required) and description (optional) and press Commit.
4. You still need to push your changes to the repository. First, run a 'pull' to see if any changes had been made since you last pulled. Then presh 'push' to submit your changes. *If you don't push, your changes aren't actually sent off of your computer*
  - If you encounter an error like [this](https://gist.github.com/budak7273/11263d4a88483189532e38d4d1d8947c), then changes have been made since you last pulled. Go back and pull from the repository, then try to push again.
  - If you encounter a conflict, you will have to resolve it before you push. Try to resolve the merge conflict by choosing which changes you want to keep from each version by editing the conflicted files. Google how to do this if you get stuck. If you can't fix it, create a new branch and push your changes to that branch instead for review later. DO NOT use 'force push.'
