# 1. Aşama: SDK ile Derleme (Build)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Proje dosyalarını kopyala
COPY ["Stajyer_Projesi/Stajyer_Projesi.csproj", "Stajyer_Projesi/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Domain/Domain.csproj", "Domain/"]

# Restore işlemi
RUN dotnet restore "Stajyer_Projesi/Stajyer_Projesi.csproj"

# Tüm kodu kopyala ve derle
COPY . .
WORKDIR "/src/Stajyer_Projesi"
RUN dotnet build "Stajyer_Projesi.csproj" -c Release -o /app/build

# --- Hata Buradaydı: 'publish' aşamasını açıkça tanımlıyoruz ---
FROM build AS publish
RUN dotnet publish "Stajyer_Projesi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 2. Aşama: Çalıştırma (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
# Artık yukarıda tanımladığımız 'publish' aşamasından dosyaları alabilir
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "Stajyer_Projesi.dll"]