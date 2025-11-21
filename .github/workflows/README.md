# GitHub Actions Workflows

## NuGet Package 배포 자동화

이 디렉토리에는 GitHub Actions 워크플로우가 포함되어 있습니다.

### `nuget-publish.yml`

master 브랜치에 push 또는 merge될 때마다 자동으로:
1. 솔루션 빌드
2. NuGet 패키지 생성 (두 프로젝트 모두)
3. NuGet.org에 자동 배포

### 설정 방법

1. **GitHub Secrets 설정**
   - GitHub 저장소로 이동
   - Settings → Secrets and variables → Actions
   - "New repository secret" 클릭
   - Name: `NUGET_API_KEY`
   - Value: NuGet.org API 키 입력
   - "Add secret" 클릭

2. **NuGet.org API 키 생성**
   - https://www.nuget.org/account/apikeys 접속
   - "Create" 클릭
   - API 키 이름 입력
   - "Select Scopes"에서 "Push" 선택
   - "Select Packages"에서 "Select specific packages" 또는 "All packages" 선택
   - "Create" 클릭
   - 생성된 API 키를 복사하여 GitHub Secrets에 저장

### 워크플로우 동작

- **트리거**: master 브랜치에 push 또는 PR이 merge될 때
- **빌드**: Release 구성으로 빌드
- **패키징**: 두 프로젝트 모두 NuGet 패키지 생성
- **배포**: NuGet.org에 자동 배포 (중복 시 건너뜀)
- **아티팩트**: 생성된 패키지를 GitHub Actions 아티팩트로 저장 (30일 보관)

### 주의사항

- API 키는 절대 코드에 하드코딩하지 마세요
- GitHub Secrets에만 저장하세요
- 패키지 버전은 `.csproj` 파일의 `<Version>` 태그에서 관리됩니다
- 동일한 버전의 패키지가 이미 존재하면 `--skip-duplicate` 옵션으로 인해 건너뜁니다

