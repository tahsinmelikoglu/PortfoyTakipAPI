pipeline {
    agent any
    stages {
        stage('Derleme (Build)') {
            steps {
                echo 'Proje derleniyor...'
                sh 'dotnet build'
            }
        }
        stage('Yayınlama (Deploy)') {
            steps {
                echo 'Uygulama arka planda ayağa kaldırılıyor...'
                sh '''
                    
                    pkill -f "PortfoyTakipAPI.dll" || true
                    
                   
                    dotnet publish -c Release -o ./publish
                    
                    
                    JENKINS_NODE_COOKIE=dontKillMe ASPNETCORE_ENVIRONMENT=Development nohup dotnet ./publish/PortfoyTakipAPI.dll --urls "http://0.0.0.0:5000" > api.log 2>&1 &
                '''
            }
        }
    }
}
