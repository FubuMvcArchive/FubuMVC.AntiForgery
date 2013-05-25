begin
  require 'bundler/setup'
  require 'fuburake'
rescue LoadError
  puts 'Bundler and all the gems need to be installed prior to running this rake script. Installing...'
  system("gem install bundler --source http://rubygems.org")
  sh 'bundle install'
  system("bundle exec rake", *ARGV)
  exit 0
end


FubuRake::Solution.new do |sln|
	sln.compile = {
		:solutionfile => 'src/FubuMVC.AntiForgery.sln'
	}
				 
	sln.assembly_info = {
		:product_name => "FubuMVC.AntiForgery",
		:copyright => 'Copyright 2011-2013 Corey Kaylor, Jason Staten, et al. All rights reserved.'
	}
	
	sln.ripple_enabled = true
	sln.fubudocs_enabled = true
end

