for %%p in (*.nupkg) do nuget push %%p -source https://www.nuget.org/api/v2/package