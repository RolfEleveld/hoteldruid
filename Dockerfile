# HotelDruid LAMP Stack Dockerfile
FROM php:8.1-apache

# Set maintainer info
LABEL maintainer="HotelDruid Docker Setup"
LABEL description="LAMP stack for HotelDruid hotel management system"

# Install system dependencies and PHP extensions
RUN apt-get update && apt-get install -y \
    libpng-dev \
    libjpeg-dev \
    libfreetype6-dev \
    libzip-dev \
    libxml2-dev \
    libonig-dev \
    default-mysql-client \
    wget \
    unzip \
    curl \
    && docker-php-ext-configure gd --with-freetype --with-jpeg \
    && docker-php-ext-install -j$(nproc) \
        gd \
        mysqli \
        pdo \
        pdo_mysql \
        zip \
        mbstring \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

# Enable Apache modules
RUN a2enmod rewrite
RUN a2enmod ssl
RUN a2enmod headers

# Configure PHP
RUN echo "upload_max_filesize = 50M" >> /usr/local/etc/php/conf.d/uploads.ini \
    && echo "post_max_size = 50M" >> /usr/local/etc/php/conf.d/uploads.ini \
    && echo "max_execution_time = 300" >> /usr/local/etc/php/conf.d/uploads.ini \
    && echo "memory_limit = 256M" >> /usr/local/etc/php/conf.d/uploads.ini \
    && echo "session.save_path = '/tmp'" >> /usr/local/etc/php/conf.d/session.ini \
    && echo "session.gc_maxlifetime = 1440" >> /usr/local/etc/php/conf.d/session.ini

# Set working directory
WORKDIR /var/www/html

# Set Docker environment variable
ENV DOCKER_ENVIRONMENT=true

# Copy HotelDruid application to web root
COPY hoteldruid/ /var/www/html/

# Create necessary directories and set permissions
RUN mkdir -p /var/www/html/dati \
    && mkdir -p /var/www/html/includes \
    && chown -R www-data:www-data /var/www/html \
    && chmod -R 755 /var/www/html \
    && chmod -R 777 /var/www/html/dati

# Create custom Apache configuration
RUN echo '<VirtualHost *:80>' > /etc/apache2/sites-available/hoteldruid.conf \
    && echo '    ServerName localhost' >> /etc/apache2/sites-available/hoteldruid.conf \
    && echo '    DocumentRoot /var/www/html' >> /etc/apache2/sites-available/hoteldruid.conf \
    && echo '    <Directory /var/www/html>' >> /etc/apache2/sites-available/hoteldruid.conf \
    && echo '        Options Indexes FollowSymLinks' >> /etc/apache2/sites-available/hoteldruid.conf \
    && echo '        AllowOverride All' >> /etc/apache2/sites-available/hoteldruid.conf \
    && echo '        Require all granted' >> /etc/apache2/sites-available/hoteldruid.conf \
    && echo '    </Directory>' >> /etc/apache2/sites-available/hoteldruid.conf \
    && echo '    ErrorLog ${APACHE_LOG_DIR}/hoteldruid_error.log' >> /etc/apache2/sites-available/hoteldruid.conf \
    && echo '    CustomLog ${APACHE_LOG_DIR}/hoteldruid_access.log combined' >> /etc/apache2/sites-available/hoteldruid.conf \
    && echo '</VirtualHost>' >> /etc/apache2/sites-available/hoteldruid.conf

# Enable the site and disable default
RUN a2ensite hoteldruid.conf && a2dissite 000-default.conf

# Create .htaccess for security (optional)
RUN echo 'RewriteEngine On' > /var/www/html/.htaccess \
    && echo 'RewriteCond %{REQUEST_FILENAME} !-f' >> /var/www/html/.htaccess \
    && echo 'RewriteCond %{REQUEST_FILENAME} !-d' >> /var/www/html/.htaccess \
    && echo 'Options -Indexes' >> /var/www/html/.htaccess \
    && echo 'ServerTokens Prod' >> /var/www/html/.htaccess

# Expose port 80
EXPOSE 80

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=30s --retries=3 \
    CMD curl -f http://localhost/ || exit 1

# Start Apache in foreground
CMD ["apache2-foreground"]