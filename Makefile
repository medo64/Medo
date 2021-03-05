.PHONY: all clean debug release test

all: release

clean:
	@./Make.sh clean

debug: clean
	@./Make.sh debug

release: clean
	@./Make.sh release

test:
	@./Make.sh test
