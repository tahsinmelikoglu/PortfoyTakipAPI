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
                echo 'Şifreler enjekte ediliyor ve uygulama ayağa kaldırılıyor...'
                
                // Jenkins kasasındaki şifreleri güvenli ortama (ENV) çekiyoruz
                withCredentials([
                    string(credentialsId: 'JENKINS_DB_SIFRESI', variable: 'ENV_DB_SIFRESI'),
                    string(credentialsId: 'JENKINS_JWT_SECRET', variable: 'ENV_JWT_SECRET')
                ]) {
sh '''
                        # 0. .NET'in kafasını karıştıran eski yayın klasörünü tamamen sil
                        rm -rf ./publish
                        
                        # 1. GitHub'dan gelen yer tutucuları, kasadaki gerçek şifrelerle değiştir
                        sed -i "s/__DB_SIFRESI__/${ENV_DB_SIFRESI}/g" appsettings.json
                        sed -i "s/__JWT_SECRET__/${ENV_JWT_SECRET}/g" appsettings.json
                        
                        # 2. Şifreler yerleştikten sonra projeyi derle ve yepyeni bir publish klasörü oluştur
                        dotnet publish -c Release -o ./publish
                        
                        # 3. Bizim kurduğumuz Linux servisini yeni kodlarla yeniden başlat
                        sudo systemctl restart portfoyapi.service
                    '''                }
            }
        }
    }
}