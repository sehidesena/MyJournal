abp install-libs

cd Mentalfull && dotnet run --migrate-database && cd -



cd Mentalfull && dotnet dev-certs https -v -ep openiddict.pfx -p eb8056da-0169-49a1-aafa-10bb2cc95e66



exit 0