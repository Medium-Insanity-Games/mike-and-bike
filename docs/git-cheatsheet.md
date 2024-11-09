# Cheat Sheet
## Status and Updates
Find out what branch you are on and what changes have been made (but not committed)
```
git status
```
Pull changes from the server (update) with
```
git pull
```
I like to do `git pull -p` since it deletes branches that have been deleted on the server.

## Move to an Existing Branch
```
git checkout [branch-name]
```
If you don't know what options are available to you, you can do
```
git branch -a
```
which will show both your local branches and the ones on the origin server (you don't need to include ```remotes/origin/``` in the name when checking those branches out).

## Creating a New Branch
```
git checkout -b [branch-name]
```
OR
```
git branch [branch-name]
git checkout [branch-name]
```

## Add Changes to a Branch
- Move to the branch
- Make changes, like adding bob.txt and bill.txt then run

```
git add bob.txt bill.txt
```
alternatively, if you want to add all the changes you've made, you can replace the file names with a period.
```
git add .
```

You then need to commit these changes and provide a message describing them.

```
git commit -m "bob and bill are my friends, so I included them here"
```

Now you need to push the new commit(s) to the server so others can see it.
```
git push
```
If it's the first time pushing changes in this branch (i.e. the server doesn't know about this new branch), you'll need to use a special command that looks like this
```
git push --set-upstream origin [branch-name]
```
but doing a normal git push will show you this command if you need to use it, so you can just copy and paste it from there (that's what I do).

## Deleting a Branch
```
git branch -d [branch-name]
```


# Example

Suppose I want to add an asset file to the project.

I would start by checking out the main branch and pulling all the new updates

```
...\mike-and-bike> git checkout main

Switched to branch 'main'
Your branch is up to date with 'origin/main'.


...\mike-and-bike> git pull -p 

Already up to date.
```
Then I would checkout my new feature branch
```
...\mike-and-bike> git checkout -b kenns/add-cool-asset

Switched to a new branch 'kenns/add-cool-asset'
```

I would then drag my files into the folder and check the status before I add/commit changes

```
...\mike-and-bike> git status

On branch kenns/add-cool-asset
Untracked files:
  (use "git add <file>..." to include in what will be committed)
        cool_asset

nothing added to commit but untracked files present (use "git add" to track)
```
I would then listen to the output and add the cool asset
```
...\mike-and-bike> git add .\cool_asset
```
and since I'm compulsive I would do another git status...
```
...\mike-and-bike> git status

On branch kenns/add-cool-asset
Changes to be committed:
  (use "git restore --staged <file>..." to unstage)
        new file:   cool_asset

Untracked files:
  (use "git add <file>..." to include in what will be committed)
        docs/


...\mike-and-bike> git commit -m "this is cool"

[kenns/add-cool-asset 802a788] this is cool
 1 file changed, 0 insertions(+), 0 deletions(-)
 create mode 100644 cool_asset
 ```
 Now, all I have to do is push it!
 ```
...\mike-and-bike> git push

fatal: The current branch kenns/add-cool-asset has no upstream branch.
To push the current branch and set the remote as upstream, use

    git push --set-upstream origin kenns/add-cool-asset

To have this happen automatically for branches without a tracking
upstream, see 'push.autoSetupRemote' in 'git help config'.
```
Ah yes, this is a new branch that the server doesn't know about, time for some copy and pasting

```
PS ...\mike-and-bike> git push --set-upstream origin kenns/add-cool-asset

Enumerating objects: 4, done.
Counting objects: 100% (4/4), done.
Delta compression using up to 12 threads
Compressing objects: 100% (2/2), done.
Writing objects: 100% (3/3), 297 bytes | 297.00 KiB/s, done.
Total 3 (delta 1), reused 0 (delta 0), pack-reused 0 (from 0)
remote: Resolving deltas: 100% (1/1), completed with 1 local object.
remote: 
remote: Create a pull request for 'kenns/add-cool-asset' on GitHub by visiting:
remote:      https://github.com/Medium-Insanity-Games/mike-and-bike/pull/new/kenns/add-cool-asset
remote:
To github.com:Medium-Insanity-Games/mike-and-bike.git
 * [new branch]      kenns/add-cool-asset -> kenns/add-cool-asset
branch 'kenns/add-cool-asset' set up to track 'origin/kenns/add-cool-asset'.
```

And there you have it, the cool new asset is published in a branch. From here I would go to github, find the branch, and make a pull request to get this merged into the main branch.