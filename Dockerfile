# syntax=docker/dockerfile:1

# ---- Build stage (Alpine SDK) ----
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish api-hrm.csproj -c Release -o /app/publish --no-restore

# ---- Runtime stage (Debian ASP.NET / glibc) ----
# NOTE: This service uses QuestPDF -> SkiaSharp, whose native library is
# built for glibc. Running it on Alpine (musl) crashes with SIGSEGV
# (exit 139), so we use the Debian-based runtime here and install
# libfontconfig1 (required by SkiaSharp for text/font rendering).
# Migrations + seeding run programmatically on startup (Program.cs
# calls Database.MigrateAsync), so no EF tooling is needed at runtime.
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends libfontconfig1 libgssapi-krb5-2 sqlite3 curl python3 python3-pip python3-venv \
    && rm -rf /var/lib/apt/lists/*

# Install Python ML dependencies directly
RUN pip3 install --no-cache-dir --break-system-packages \
    pandas>=2.0.0 \
    numpy>=1.24.0 \
    scikit-learn==1.6.1 \
    joblib>=1.3.0

ENV ASPNETCORE_URLS=http://0.0.0.0:5000
EXPOSE 5000

COPY --from=build /app/publish ./
CMD ["dotnet", "api-hrm.dll"]