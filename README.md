# Remote Radar Setting Tool

원격으로 레이더의 설정을 변경하고 예약할 수 있는 윈도우 기반 도구입니다.

---

## 디버그 방법

bash
dotnet run


---

## 빌드 방법

bash
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true


- 실행 파일 경로:  
  bin\Release\net9.0-windows\win-x64\publish\radar_settinf_tool_project.exe
   
---
main Folder/
├── radar_settinf_tool_project.exe
└── json/
    └── yn_UidList_2.json


## 🛠️ 사용 방법

### 1. 로그인

프로그램 실행 후 로그인 화면에서 사용자 정보를 입력합니다.

### 2. 모드 선택

- **개별 세팅**: 1대1 레이더 설정
- **통합 세팅**: 해당 층에 설치된 모든 레이더를 동시에 설정

---

### 3. 세팅 방법

#### ✅ 개별 세팅

1. **Port**와 **UID** 입력  
2. Connect to Server 버튼 클릭  
3. **COMMAND**와 **VALUE** 입력  
4. Send 버튼 클릭  

#### ✅ 통합 세팅

1. 해당 층의 uid_list.json 파일 선택  
2. **COMMAND**와 **VALUE** 입력  
3. Send 버튼 클릭  
4. 실패한 레이더에 대해 **예약 세팅** 가능  

---

### 4. 예약 세팅

1. 예약 시간 선택  
2. 확인 버튼 클릭

---
## 📁 참고

- 설정은 내부적으로 TCP 통신을 통해 레이더에 명령을 전송합니다.
- uid_list.json 파일은 층별 레이더 UID 정보를 담고 있어야 합니다.
   - 각 층에 대하 레이더 UID 정보는 없을 경우 추가를 해야 하고 형식은 다음과 같습니다.
      - 파일 이름 : 병원이름_UidList_층.json
      - 파일 형식 : { "호실_침대" : "uid" : "Topici_d","port": 포트 번호 }  
         -    예시: {"211_1": { "uid": "21b7/0559E31020B1A", "port": 18092 },}
       # Remote Radar Setting Tool

원격으로 레이더의 설정을 변경하고 예약할 수 있는 윈도우 기반 도구입니다.

---

## 디버그 방법

bash
dotnet run


---

## 빌드 방법

bash
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true


- 실행 파일 경로:  
  bin\Release\net9.0-windows\win-x64\publish\radar_settinf_tool_project.exe
   
---
main Folder/
├── radar_settinf_tool_project.exe
└── json/
    └── yn_UidList_2.json


## 🛠️ 사용 방법

### 1. 로그인

프로그램 실행 후 로그인 화면에서 사용자 정보를 입력합니다.

### 2. 모드 선택

- **개별 세팅**: 1대1 레이더 설정
- **통합 세팅**: 해당 층에 설치된 모든 레이더를 동시에 설정

---

### 3. 세팅 방법

#### ✅ 개별 세팅

1. **Port**와 **UID** 입력  
2. Connect to Server 버튼 클릭  
3. **COMMAND**와 **VALUE** 입력  
4. Send 버튼 클릭  

#### ✅ 통합 세팅

1. 해당 층의 uid_list.json 파일 선택  
2. **COMMAND**와 **VALUE** 입력  
3. Send 버튼 클릭  
4. 실패한 레이더에 대해 **예약 세팅** 가능  

---

### 4. 예약 세팅

1. 예약 시간 선택  
2. 확인 버튼 클릭

---
## 📁 참고

- 설정은 내부적으로 TCP 통신을 통해 레이더에 명령을 전송합니다.
- uid_list.json 파일은 층별 레이더 UID 정보를 담고 있어야 합니다.
   - 각 층에 대하 레이더 UID 정보는 없을 경우 추가를 해야 하고 형식은 다음과 같습니다.
      - 파일 이름 : 병원이름_UidList_층.json
      - 파일 형식 : { "호실_침대" : "uid" : "Topici_d","port": 포트 번호 }  
         -    예시: {"211_1": { "uid": "21b7/0559E31020B1A", "port": 18092 },}
