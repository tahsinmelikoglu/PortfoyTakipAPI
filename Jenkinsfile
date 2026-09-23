pipeline {
    agent any
    stages {
        stage('Derleme (Build)') {
            steps {
                echo 'Proje derleniyor...'
                sh 'dotnet build'
            }
        }
    }
}
